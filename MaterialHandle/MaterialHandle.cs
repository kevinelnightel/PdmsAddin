// ╔══════════════════════════════════════════════════════════════════════════════╗
// ║                     MaterialHandle — 材料清单提取工具                          ║
// ╠══════════════════════════════════════════════════════════════════════════════╣
// ║                                                                              ║
// ║  【是什么】一个独立的控制台 exe，解析 PDMS ISO 材料清单文件，生成 Excel。       ║
// ║  【兼容框架】.NET Framework 4.8                                               ║
// ║  【依赖 NuGet】ClosedXML 0.105.0（已内置在 bin 目录中）                        ║
// ║                                                                              ║
// ║  【部署注意】必须把 bin\Debug\net48\ 下所有文件一起拷贝到目标目录！               ║
// ║    不能只拷贝 MaterialHandle.exe，否则会报"未能加载 ClosedXML"错误。            ║
// ║                                                                              ║
// ║  ── 接口①：解析单根管道材料（输出 JSON 到文件）────────────────────────────    ║
// ║  命令行：                                                                     ║
// ║    MaterialHandle.exe --parse <材料文件路径> <输出JSON文件路径>                   ║
// ║  示例：                                                                       ║
// ║    MaterialHandle.exe --parse "D:\PDMSISO\MTO\matl" "D:\output\pipe1.json"     ║
// ║  输出到文件：UTF-8 格式的 JSON 数组                                             ║
// ║                                                                              ║
// ║  ── 接口②：根据 JSON 生成 Excel ──────────────────────────────────────────    ║
// ║  命令行：                                                                     ║
// ║    MaterialHandle.exe --generate <JSON文件路径> <输出目录>                     ║
// ║  示例：                                                                       ║
// ║    MaterialHandle.exe --generate "D:\output\all_items.json" "D:\输出"          ║
// ║  效果：在输出目录下生成"单线材料表.xlsx"和"汇总材料表.xlsx"                    ║
// ║                                                                              ║
// ║  ── C# 调用示例（你不需要引用任何 DLL）────────────────────────────────────    ║
// ║                                                                              ║
// ║  // 用 List<string> 收集每根管道生成的 JSON 文件路径                              ║
// ║  var jsonFiles = new List<string>();                                          ║
// ║                                                                              ║
// ║  foreach (var pipeFile in pipeFiles)                                         ║
// ║  {                                                                           ║
// ║      string outJson = @"D:\temp\pipe_" + index + ".json";                     ║
// ║      jsonFiles.Add(outJson);                                                 ║
// ║      // 接口①：解析一根管道，写 JSON 到文件                                      ║
// ║      var psi = new ProcessStartInfo                                          ║
// ║      {                                                                       ║
// ║          FileName = @"D:\Tools\MaterialHandle.exe",                          ║
// ║          Arguments = "--parse \"" + pipeFile + "\" \"" + outJson + "\"",     ║
// ║          UseShellExecute = false,                                            ║
// ║          CreateNoWindow = true                                               ║
// ║      };                                                                      ║
// ║      var p = Process.Start(psi);                                             ║
// ║      p.WaitForExit();                                                        ║
// ║  }                                                                           ║
// ║                                                                              ║
// ║  // 全部解析完，拼接成一个大 JSON 数组                                           ║
// ║  var sb = new StringBuilder();                                               ║
// ║  sb.Append("[");                                                             ║
// ║  for (int i = 0; i < jsonFiles.Count; i++)                                   ║
// ║  {                                                                           ║
// ║      var t = File.ReadAllText(jsonFiles[i]).Trim();                           ║
// ║      if (t.StartsWith("[")) t = t.Substring(1);                              ║
// ║      if (t.EndsWith("]"))   t = t.Substring(0, t.Length - 1);                ║
// ║      if (!string.IsNullOrEmpty(t))                                           ║
// ║      { if (i > 0) sb.Append(","); sb.Append(t); }                            ║
// ║  }                                                                           ║
// ║  sb.Append("]");                                                             ║
// ║  File.WriteAllText(@"D:\temp\all.json", sb.ToString());                       ║
// ║                                                                              ║
// ║  // 接口②：生成 Excel                                                         ║
// ║  var psi2 = new ProcessStartInfo                                             ║
// ║  {                                                                           ║
// ║      FileName = @"D:\Tools\MaterialHandle.exe",                              ║
// ║      Arguments = "--generate \"D:\\temp\\all.json\" \"D:\\输出\"",             ║
// ║      UseShellExecute = false,                                                ║
// ║      CreateNoWindow = true                                                   ║
// ║  };                                                                          ║
// ║  var p2 = Process.Start(psi2);                                               ║
// ║  p2.WaitForExit();                                                           ║
// ║  if (p2.ExitCode != 0) { /* 失败 */ }                                         ║
// ║                                                                              ║
// ║  生成的 Excel：                                                               ║
// ║    outputDir\单线材料表.xlsx  → Sheet1 总表 + 各管线独立分表                   ║
// ║    outputDir\汇总材料表.xlsx  → 跨管线合并同类同规格                           ║
// ║                                                                              ║
// ╚══════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace MaterialHandle
{

    // ========== 数据模型 ==========
    public class MaterialItem
    {
        public string PipelineNo { get; set; }
        public string MaterialType { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }

    // ========== TXT 解析器 ==========

    class TxtParser
    {
        private static readonly Regex PipelineRefRegex = new Regex(
            @"PIPELINE REF\s+(.+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex DataLineRegex = new Regex(
            @"^\s+(\d{1,3})\s{2,}",
            RegexOptions.Compiled);

        private static readonly Regex SkipRegex = new Regex(
            @"^\s*$|序号|^\-{3,}|PAGE \d|^\d{2}/\d{2}/\d{4}|CUT PIPE LENGTH|PIECE\s+CUT|^\s*<",
            RegexOptions.Compiled);

        private static readonly Dictionary<string, string> CategoryMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
        { "PIPE",                   "PIPE" },
        { "FITTINGS",               "FITTING" },
        { "FLANGES",                "FLANGE" },
        { "VALVES / IN-LINE ITEMS","VALV" },
        { "GASKETS",                "GASKET" },
        { "BOLTS",                  "BOLTS" },
        { "INSTRUMENTS",            "INSTRUMENT" },
        { "SUPPORTS",               "SUPPORT" },
        };

        public List<MaterialItem> Parse(string filePath)
        {
            var items = new List<MaterialItem>();
            var lines = File.ReadAllLines(filePath);

            string currentPipeline = "";
            string currentCategory = "";

            foreach (var rawLine in lines)
            {
                var pipeMatch = PipelineRefRegex.Match(rawLine);
                if (pipeMatch.Success)
                {
                    var pipeNo = pipeMatch.Groups[1].Value.Trim();
                    currentPipeline = Regex.Replace(pipeNo, @"\s+\d+$", "");
                    continue;
                }

                if (SkipRegex.IsMatch(rawLine))
                    continue;

                var trimmed = rawLine.Trim();
                if (trimmed.Equals("FABRICATION MATERIALS", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("ERECTION MATERIALS", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (CategoryMap.TryGetValue(trimmed, out var normalizedType))
                {
                    currentCategory = normalizedType;
                    continue;
                }

                if (DataLineRegex.IsMatch(rawLine))
                {
                    var item = ParseDataLine(rawLine);
                    if (item != null)
                    {
                        item.PipelineNo = currentPipeline;
                        item.MaterialType = currentCategory;
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        private MaterialItem ParseDataLine(string line)
        {
            var parts = Regex.Split(line, @"\s{2,}")
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .ToArray();

            if (parts.Length < 3)
                return null;

            var meaningful = parts.Skip(1).ToArray();
            int lastIdx = meaningful.Length - 1;

            // Quantity
            string quantityRaw = meaningful[lastIdx].Trim();
            var qtyUnit = ParseQuantity(quantityRaw);
            decimal qty = qtyUnit.qty;
            string unit = qtyUnit.unit;

            // ITEM CODE / Bore
            string itemCode = "", bore = "";
            bool boreWasMerged = false;

            string secondLast = lastIdx - 1 >= 0 ? meaningful[lastIdx - 1].Trim() : null;
            string thirdLast = lastIdx - 2 >= 0 ? meaningful[lastIdx - 2].Trim() : null;

            if (secondLast != null && IsItemCode(secondLast))
            {
                var split = SplitBoreItemCode(secondLast);
                if (split.borePart != null)
                {
                    bore = split.borePart;
                    itemCode = split.codePart;
                    boreWasMerged = true;
                }
                else
                {
                    itemCode = secondLast;
                    bore = thirdLast ?? "";
                }
            }
            else if (secondLast != null && IsBore(secondLast))
            {
                var split = SplitBoreItemCode(secondLast);
                if (split.codePart != null)
                {
                    bore = split.borePart;
                    itemCode = split.codePart;
                    boreWasMerged = true;
                }
                else
                {
                    bore = secondLast;
                    itemCode = "";
                }
            }

            // Description
            int descEndIdx;
            if (!string.IsNullOrEmpty(itemCode))
                descEndIdx = lastIdx - (boreWasMerged ? 2 : 3);
            else if (!string.IsNullOrEmpty(bore))
                descEndIdx = lastIdx - 2;
            else
                descEndIdx = lastIdx - 1;

            string description = descEndIdx >= 0
                ? string.Join("  ", meaningful.Take(descEndIdx + 1)).Trim()
                : "";

            return new MaterialItem
            {
                Description = description,
                Specification = bore,
                ItemCode = itemCode,
                Quantity = qty,
                Unit = unit,
            };
        }

        private bool IsItemCode(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            if (text.Contains("-") || (text.Contains("/") && !Regex.IsMatch(text, @"^\d+/\d+$")))
                return true;
            return Regex.IsMatch(text, @"[A-Z]+[\-_/][A-Z0-9]");
        }

        private (string borePart, string codePart) SplitBoreItemCode(string text)
        {
            var match = Regex.Match(text, @"^([\d.]+/(?:[\d.]+)?\s*x\s*[\d.]+(?:/[\d.]+)?)\s+([A-Z][A-Za-z0-9\-_/]+)$");
            if (match.Success)
                return (match.Groups[1].Value, match.Groups[2].Value);
            return (null, null);
        }

        private bool IsBore(string text)
        {
            if (string.IsNullOrEmpty(text)) return true;
            if (Regex.IsMatch(text, @"^\d+$")) return true;
            if (Regex.IsMatch(text, @"^\d+/\d+$")) return true;
            if (Regex.IsMatch(text, @"^\d+\s*x\s*\d+/\d+$")) return true;
            if (Regex.IsMatch(text, @"^\d+\s*x\s*\d+$")) return true;
            if (Regex.IsMatch(text, @"^\d+/\d+\""?$")) return true;
            return false;
        }

        private (decimal qty, string unit) ParseQuantity(string raw)
        {
            var match = Regex.Match(raw.Trim(), @"^([\d.]+)([A-Za-z]*)$");
            if (match.Success)
            {
                decimal qty = decimal.Parse(match.Groups[1].Value);
                string unit = match.Groups[2].Value;
                if (string.IsNullOrEmpty(unit)) unit = "PCS";
                return (qty, unit);
            }
            return (0, "PCS");
        }
    }

    // ========== Excel 生成器 ==========

    class ExcelWriter
    {
        private static readonly Dictionary<string, int> TypeOrder =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
        { "PIPE",       1 },
        { "FITTING",    2 },
        { "VALV",       3 },
        { "FLANGE",     4 },
        { "GASKET",     5 },
        { "INSTRUMENT", 6 },
        { "BOLTS",      7 },
        { "SUPPORT",    8 },
        };

        private static readonly string[] PipeHeaders =
            { "序号", "材料描述", "材料类型", "材料规格", "材料代码", "数量", "单位", "备注" };

        private static readonly string[] SumHeaders =
            { "序号", "材料描述", "材料类型", "材料规格", "材料代码", "数量", "单位", "备注", "涉及管线" };

        private static readonly double[] PipeColWidths = { 6, 55, 12, 14, 22, 10, 8, 15 };
        private static readonly double[] SumColWidths = { 6, 55, 12, 14, 22, 10, 8, 15, 40 };

        private static int GetTypeOrder(string type)
        {
            if (TypeOrder.TryGetValue(type, out int order))
                return order;
            return 99;
        }

        public void WriteSingleLineTable(List<MaterialItem> items, string outputPath)
        {
            using (var wb = new XLWorkbook())
            {
                var groups = items.GroupBy(i => i.PipelineNo).OrderBy(g => g.Key).ToList();

                while (wb.Worksheets.Count > 0)
                    wb.Worksheets.First().Delete();

                // 总表
                {
                    var sheet = wb.Worksheets.Add("总表");
                    var masterHeaders = new[] { "序号", "管线号", "材料描述", "材料类型", "材料规格", "材料代码", "数量", "单位", "备注" };
                    sheet.Cell(1, 1).Value = "单线材料总表";
                    sheet.Range(1, 1, 1, masterHeaders.Length).Merge().Style.Font.Bold = true;
                    WriteHeader(sheet, 3, masterHeaders);

                    int r = 4, seq = 0;
                    foreach (var pipeGroup in groups)
                        foreach (var m in pipeGroup.OrderBy(i => GetTypeOrder(i.MaterialType))
                                                    .ThenBy(i => i.Description))
                        {
                            seq++;
                            sheet.Cell(r, 1).Value = seq;
                            sheet.Cell(r, 2).Value = pipeGroup.Key;
                            sheet.Cell(r, 3).Value = m.Description;
                            sheet.Cell(r, 4).Value = m.MaterialType;
                            sheet.Cell(r, 5).Value = m.Specification;
                            sheet.Cell(r, 6).Value = m.ItemCode;
                            sheet.Cell(r, 7).Value = m.Quantity;
                            sheet.Cell(r, 8).Value = m.Unit;
                            sheet.Cell(r, 9).Value = "";
                            r++;
                        }

                    SetColWidths(sheet, new double[] { 6, 30, 55, 12, 14, 22, 10, 8, 15 });
                }

                // 各管线分表
                foreach (var g in groups)
                {
                    var sheet = wb.Worksheets.Add(SanitizeSheetName(g.Key));

                    var merged = g
                        .GroupBy(i => new { i.MaterialType, i.Description, i.Specification, i.ItemCode, i.Unit })
                        .Select(m => new {
                            m.Key.MaterialType,
                            m.Key.Description,
                            m.Key.Specification,
                            m.Key.ItemCode,
                            m.Key.Unit,
                            TotalQty = m.Sum(x => x.Quantity)
                        })
                        .ToList();

                    sheet.Cell(1, 1).Value = "单线材料表";
                    sheet.Range(1, 1, 1, PipeHeaders.Length).Merge().Style.Font.Bold = true;
                    sheet.Cell(2, 1).Value = "管线号: " + g.Key;
                    WriteHeader(sheet, 4, PipeHeaders);

                    WriteTypedRows(sheet, 5, merged, m => m.MaterialType,
                        (r, idx, m) => {
                            sheet.Cell(r, 1).Value = idx;
                            sheet.Cell(r, 2).Value = m.Description;
                            sheet.Cell(r, 3).Value = m.MaterialType;
                            sheet.Cell(r, 4).Value = m.Specification;
                            sheet.Cell(r, 5).Value = m.ItemCode;
                            sheet.Cell(r, 6).Value = m.TotalQty;
                            sheet.Cell(r, 7).Value = m.Unit;
                            sheet.Cell(r, 8).Value = "";
                        });
                }

                wb.SaveAs(outputPath);
            }
        }

        public void WriteSummaryTable(List<MaterialItem> items, string outputPath)
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("汇总材料表");

                var merged = items
                    .GroupBy(i => new { i.MaterialType, i.Description, i.Specification, i.ItemCode, i.Unit })
                    .Select(g => new {
                        g.Key.MaterialType,
                        g.Key.Description,
                        g.Key.Specification,
                        g.Key.ItemCode,
                        g.Key.Unit,
                        TotalQty = g.Sum(x => x.Quantity),
                        Pipelines = string.Join(", ", g.Select(x => x.PipelineNo).Distinct().OrderBy(p => p))
                    })
                    .ToList();

                ws.Cell(1, 1).Value = "汇总材料表";
                ws.Range(1, 1, 1, SumHeaders.Length).Merge().Style.Font.Bold = true;
                WriteHeader(ws, 3, SumHeaders);

                WriteTypedRows(ws, 4, merged, m => m.MaterialType,
                    (r, idx, m) => {
                        ws.Cell(r, 1).Value = idx;
                        ws.Cell(r, 2).Value = m.Description;
                        ws.Cell(r, 3).Value = m.MaterialType;
                        ws.Cell(r, 4).Value = m.Specification;
                        ws.Cell(r, 5).Value = m.ItemCode;
                        ws.Cell(r, 6).Value = m.TotalQty;
                        ws.Cell(r, 7).Value = m.Unit;
                        ws.Cell(r, 8).Value = "";
                        ws.Cell(r, 9).Value = m.Pipelines;
                    });

                wb.SaveAs(outputPath);
            }
        }

        private void WriteTypedRows<T>(IXLWorksheet ws, int startRow, List<T> data,
            Func<T, string> typeSelector, Action<int, int, T> writeRow)
        {
            var grouped = data
                .OrderBy(m => GetTypeOrder(typeSelector(m)))
                .ThenBy(m => typeSelector(m))
                .GroupBy(typeSelector)
                .ToList();

            int row = startRow;

            foreach (var grp in grouped)
            {
                if (row > startRow)
                    row++;

                var titleCell = ws.Cell(row, 1);
                titleCell.Value = grp.Key;
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Fill.BackgroundColor = XLColor.LightCyan;
                ws.Range(row, 1, row, 10).Merge().Style.Fill.BackgroundColor = XLColor.LightCyan;
                row++;

                int idx = 0;
                var sorted = grp.OrderBy(m => {
                    var prop = typeof(T).GetProperty("Description");
                    var val = prop?.GetValue(m);
                    return val?.ToString() ?? "";
                }, StringComparer.OrdinalIgnoreCase);
                foreach (var m in sorted)
                {
                    idx++;
                    writeRow(row, idx, m);
                    row++;
                }
            }

            SetColWidths(ws, ws.ColumnsUsed().Count() <= 8 ? PipeColWidths : SumColWidths);
        }

        private void WriteHeader(IXLWorksheet ws, int row, string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                var c = ws.Cell(row, i + 1);
                c.Value = headers[i];
                c.Style.Font.Bold = true;
                c.Style.Fill.BackgroundColor = XLColor.LightGray;
                c.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
        }

        private void SetColWidths(IXLWorksheet ws, double[] widths)
        {
            for (int i = 0; i < widths.Length; i++)
                ws.Column(i + 1).Width = widths[i];
        }

        private string SanitizeSheetName(string name)
        {
            foreach (var c in new[] { '[', ']', '*', '?', ':', '/', '\\', '\'' })
                name = name.Replace(c.ToString(), "-");
            return name.Length > 31 ? name.Substring(0, 31) : name;
        }
    }

    // ========== 简易 JSON 序列化（无需第三方库） ==========

    static class JsonHelper
    {
        /// <summary>将 MaterialItem 列表序列化为 JSON 数组字符串</summary>
        public static string Serialize(List<MaterialItem> items)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(SerializeItem(items[i]));
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>从 JSON 数组字符串反序列化为 MaterialItem 列表</summary>
        public static List<MaterialItem> Deserialize(string json)
        {
            var items = new List<MaterialItem>();
            if (string.IsNullOrEmpty(json)) return items;

            // 找到第一个 [ 和最后一个 ]
            int start = json.IndexOf('[');
            int end = json.LastIndexOf(']');
            if (start < 0 || end < 0 || end <= start) return items;

            string inner = json.Substring(start + 1, end - start - 1);

            // 按 },{ 分割每个对象
            int depth = 0, segStart = 0;
            for (int i = 0; i < inner.Length; i++)
            {
                if (inner[i] == '{') depth++;
                else if (inner[i] == '}') depth--;
                else if (inner[i] == ',' && depth == 0)
                {
                    items.Add(DeserializeItem(inner.Substring(segStart, i - segStart)));
                    segStart = i + 1;
                }
            }
            // 最后一个
            if (segStart < inner.Length)
            {
                var last = inner.Substring(segStart).Trim();
                if (last.Length > 0)
                    items.Add(DeserializeItem(last));
            }

            return items;
        }

        private static string SerializeItem(MaterialItem m)
        {
            return "{"
                + JsonProp("PipelineNo", m.PipelineNo)
                + JsonProp("MaterialType", m.MaterialType)
                + JsonProp("Description", m.Description)
                + JsonProp("Specification", m.Specification)
                + JsonProp("ItemCode", m.ItemCode)
                + "\"Quantity\":" + m.Quantity.ToString("0.##") + ","
                + JsonProp("Unit", m.Unit)
                + "}";
        }

        private static string JsonProp(string name, string value)
        {
            return "\"" + name + "\":\"" + JsonEscape(value ?? "") + "\",";
        }

        private static string JsonEscape(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        private static MaterialItem DeserializeItem(string segment)
        {
            var m = new MaterialItem();
            if (string.IsNullOrEmpty(segment)) return m;

            // 去掉首尾 {}
            segment = segment.Trim();
            if (segment.StartsWith("{")) segment = segment.Substring(1);
            if (segment.EndsWith("}")) segment = segment.Substring(0, segment.Length - 1);

            foreach (var kv in SplitJson(segment))
            {
                int colon = kv.IndexOf(':');
                if (colon <= 0) continue;
                string key = kv.Substring(0, colon).Trim().Trim('"');
                string val = kv.Substring(colon + 1).Trim().Trim('"');

                switch (key)
                {
                    case "PipelineNo": m.PipelineNo = Unescape(val); break;
                    case "MaterialType": m.MaterialType = Unescape(val); break;
                    case "Description": m.Description = Unescape(val); break;
                    case "Specification": m.Specification = Unescape(val); break;
                    case "ItemCode": m.ItemCode = Unescape(val); break;
                    case "Unit": m.Unit = Unescape(val); break;
                    case "Quantity":
                        decimal q;
                        if (decimal.TryParse(val, out q)) m.Quantity = q;
                        break;
                }
            }
            return m;
        }

        private static List<string> SplitJson(string segment)
        {
            var result = new List<string>();
            bool inString = false;
            int depth = 0, start = 0;
            for (int i = 0; i < segment.Length; i++)
            {
                char c = segment[i];
                if (c == '"' && (i == 0 || segment[i - 1] != '\\')) inString = !inString;
                else if (!inString)
                {
                    if (c == '{' || c == '[') depth++;
                    else if (c == '}' || c == ']') depth--;
                    else if (c == ',' && depth == 0)
                    {
                        result.Add(segment.Substring(start, i - start));
                        start = i + 1;
                    }
                }
            }
            if (start < segment.Length)
                result.Add(segment.Substring(start));
            return result;
        }

        private static string Unescape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\\\", "\\");
        }
    }

    // ========== Main ==========

    class Program
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            string mode = args[0].ToLowerInvariant();

            try
            {
                switch (mode)
                {
                    case "--parse":
                        return DoParse(args);

                    case "--generate":
                        return DoGenerate(args);

                    default:
                        // 兼容旧用法: MaterialHandle.exe <文件路径> [输出目录]
                        return DoLegacy(args);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[错误] " + ex.Message);
                return 1;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("=== MaterialHandle 材料清单提取工具 ===");
            Console.WriteLine();
            Console.WriteLine("接口① 解析单根管道材料（输出 JSON 到文件）：");
            Console.WriteLine("  MaterialHandle.exe --parse <材料文件路径> <输出JSON文件路径>");
            Console.WriteLine();
            Console.WriteLine("接口② 根据 JSON 生成 Excel：");
            Console.WriteLine("  MaterialHandle.exe --generate <JSON文件路径> <输出目录>");
            Console.WriteLine();
            Console.WriteLine("兼容旧用法（一步完成）：");
            Console.WriteLine("  MaterialHandle.exe <材料文件路径> [输出目录]");
        }

        /// <summary>
        /// 接口① --parse：解析单根管道的材料文件，输出 JSON 到文件
        /// </summary>
        static int DoParse(string[] args)
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine("[错误] --parse 需要: <材料文件路径> <输出JSON文件路径>");
                return 1;
            }

            string inputPath = args[1];
            string outputPath = args[2];

            if (!File.Exists(inputPath))
            {
                Console.Error.WriteLine("[错误] 文件不存在: " + inputPath);
                return 1;
            }

            var parser = new TxtParser();
            var items = parser.Parse(inputPath);

            string json = JsonHelper.Serialize(items);
            string dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(outputPath, json, Encoding.UTF8);
            Console.WriteLine("[完成] " + items.Count + " 条记录 -> " + outputPath);

            return 0;
        }

        /// <summary>
        /// 接口② --generate：读取 JSON 文件，生成 Excel
        /// </summary>
        static int DoGenerate(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("[错误] --generate 需要指定 JSON 文件路径");
                return 1;
            }

            string jsonPath = args[1];
            string outputDir = args.Length >= 3 ? args[2] : Path.GetDirectoryName(jsonPath) ?? ".";

            if (!File.Exists(jsonPath))
            {
                Console.Error.WriteLine("[错误] JSON 文件不存在: " + jsonPath);
                return 1;
            }

            string json = File.ReadAllText(jsonPath, Encoding.UTF8);
            var items = JsonHelper.Deserialize(json);

            if (items.Count == 0)
            {
                Console.Error.WriteLine("[错误] JSON 中没有材料数据");
                return 1;
            }

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var writer = new ExcelWriter();
            writer.WriteSingleLineTable(items, Path.Combine(outputDir, "单线材料表.xlsx"));
            Console.WriteLine("[生成] 单线材料表.xlsx");
            writer.WriteSummaryTable(items, Path.Combine(outputDir, "汇总材料表.xlsx"));
            Console.WriteLine("[生成] 汇总材料表.xlsx");
            Console.WriteLine("[完成] " + items.Count + " 条记录, "
                + items.Select(i => i.PipelineNo).Distinct().Count() + " 条管线");

            return 0;
        }

        /// <summary>
        /// 兼容旧用法：MaterialHandle.exe <文件> [输出目录]
        /// </summary>
        static int DoLegacy(string[] args)
        {
            string inputPath = args[0];
            string outputDir = args.Length >= 2
                ? args[1]
                : Path.GetDirectoryName(inputPath) ?? ".";

            if (!File.Exists(inputPath))
            {
                Console.Error.WriteLine("[错误] 文件不存在: " + inputPath);
                return 1;
            }

            var parser = new TxtParser();
            var items = parser.Parse(inputPath);

            var writer = new ExcelWriter();
            writer.WriteSingleLineTable(items, Path.Combine(outputDir, "单线材料表.xlsx"));
            Console.WriteLine("[生成] 单线材料表.xlsx");
            writer.WriteSummaryTable(items, Path.Combine(outputDir, "汇总材料表.xlsx"));
            Console.WriteLine("[生成] 汇总材料表.xlsx");
            Console.WriteLine("[完成] " + items.Count + " 条记录, "
                + items.Select(i => i.PipelineNo).Distinct().Count() + " 条管线");

            return 0;
        }
    }

} // namespace MaterialHandle
