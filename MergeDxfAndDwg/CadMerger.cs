using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

using ACadSharp;
using ACadSharp.IO;
using ACadSharp.Entities;
using ACadSharp.Tables;

namespace DxfToDwgPdmsPolylineFix
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("DxfToDwgPdmsPolylineFix.exe <source.dxf> <target.dwg>");
                return 1;
            }

            string dxfPath = args[0];
            string dwgPath = args[1];

            if (!File.Exists(dxfPath))
            {
                Console.WriteLine("DXF 文件不存在: " + dxfPath);
                return 2;
            }

            if (!File.Exists(dwgPath))
            {
                Console.WriteLine("DWG 文件不存在: " + dwgPath);
                return 3;
            }

            try
            {
                CadDocument dxfDoc = DxfReader.Read(dxfPath);
                CadDocument dwgDoc = DwgReader.Read(dwgPath);

                // 1. 合并线型表
                MergeLineTypes(dxfDoc, dwgDoc);

                // 2. 合并图层
                MergeLayers(dxfDoc, dwgDoc);

                int copiedNormalEntityCount = 0;
                int skippedOldPolylineCount = 0;

                // 3. 普通实体复制
                foreach (Entity srcEntity in dxfDoc.Entities)
                {
                    if (IsOldPolyline(srcEntity))
                    {
                        skippedOldPolylineCount++;
                        continue;
                    }

                    Entity dstEntity = (Entity)srcEntity.Clone();

                    FixNormalEntityDisplay(srcEntity, dstEntity, dwgDoc);

                    dwgDoc.Entities.Add(dstEntity);

                    copiedNormalEntityCount++;
                }

                // 4. 从原始 DXF 文本解析老式 POLYLINE
                List<RawOldPolyline> oldPolylines =
                    ParseAllOldPolylinesFromRawDxf(dxfPath);

                // 5. 重建老式 POLYLINE 为 LwPolyline
                int rebuiltPolylineCount =
                    AddOldPolylinesToDwg(oldPolylines, dwgDoc);

                // 6. 输出 DWG
                string outDir = Path.GetDirectoryName(dxfPath);

                if (string.IsNullOrWhiteSpace(outDir))
                {
                    outDir = AppDomain.CurrentDomain.BaseDirectory;
                }

                string outName = Path.GetFileNameWithoutExtension(dxfPath) + ".dwg";
                string outputDwgPath = Path.Combine(outDir, outName);

                DwgWriter.Write(outputDwgPath, dwgDoc);

                Console.WriteLine("处理完成: " + outputDwgPath);
                Console.WriteLine("普通实体复制数量: " + copiedNormalEntityCount);
                Console.WriteLine("跳过老式 POLYLINE 数量: " + skippedOldPolylineCount);
                Console.WriteLine("重建老式 POLYLINE 数量: " + rebuiltPolylineCount);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("处理失败:");
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        static bool IsOldPolyline(Entity entity)
        {
            if (entity == null)
                return false;

            string typeName = entity.GetType().Name ?? "";
            string objectName = entity.ObjectName ?? "";

            if (typeName.IndexOf("LwPolyline", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;

            if (objectName.Equals("POLYLINE", StringComparison.OrdinalIgnoreCase))
                return true;

            if (typeName.IndexOf("Polyline", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }

        static List<RawOldPolyline> ParseAllOldPolylinesFromRawDxf(string dxfPath)
        {
            string[] lines = File.ReadAllLines(dxfPath);

            List<RawOldPolyline> result = new List<RawOldPolyline>();

            int i = 0;

            while (i < lines.Length - 1)
            {
                string code = lines[i].Trim();
                string value = lines[i + 1].Trim();

                if (code == "0" &&
                    value.Equals("POLYLINE", StringComparison.OrdinalIgnoreCase))
                {
                    RawOldPolyline polyline = new RawOldPolyline();

                    i += 2;

                    while (i < lines.Length - 1)
                    {
                        string c = lines[i].Trim();
                        string v = lines[i + 1].Trim();

                        if (c == "0")
                            break;

                        switch (c)
                        {
                            case "8":
                                polyline.LayerName = v;
                                break;

                            case "6":
                                polyline.LineTypeName = v;
                                break;

                            case "62":
                                {
                                    int colorIndex;
                                    if (int.TryParse(v, out colorIndex))
                                    {
                                        polyline.ColorIndex = colorIndex;
                                    }

                                    break;
                                }

                            case "40":
                                polyline.StartWidth = ToDouble(v);
                                break;

                            case "41":
                                polyline.EndWidth = ToDouble(v);
                                break;

                            case "48":
                                polyline.LineTypeScale = ToDouble(v);
                                break;

                            case "70":
                                {
                                    int flag;
                                    if (int.TryParse(v, out flag))
                                    {
                                        polyline.Flags = flag;
                                        polyline.IsClosed = (flag & 1) == 1;
                                        polyline.Plinegen = (flag & 128) == 128;
                                    }

                                    break;
                                }

                            case "38":
                                polyline.Elevation = ToDouble(v);
                                break;

                            case "39":
                                polyline.Thickness = ToDouble(v);
                                break;
                        }

                        i += 2;
                    }

                    while (i < lines.Length - 1)
                    {
                        string c = lines[i].Trim();
                        string v = lines[i + 1].Trim();

                        if (c == "0" &&
                            v.Equals("SEQEND", StringComparison.OrdinalIgnoreCase))
                        {
                            i += 2;
                            break;
                        }

                        if (c == "0" &&
                            v.Equals("VERTEX", StringComparison.OrdinalIgnoreCase))
                        {
                            RawOldPolylineVertex vertex = new RawOldPolylineVertex();

                            i += 2;

                            while (i < lines.Length - 1)
                            {
                                string vc = lines[i].Trim();
                                string vv = lines[i + 1].Trim();

                                if (vc == "0")
                                    break;

                                switch (vc)
                                {
                                    case "8":
                                        vertex.LayerName = vv;
                                        break;

                                    case "10":
                                        vertex.X = ToDouble(vv);
                                        break;

                                    case "20":
                                        vertex.Y = ToDouble(vv);
                                        break;

                                    case "30":
                                        vertex.Z = ToDouble(vv);
                                        break;

                                    case "40":
                                        vertex.StartWidth = ToDouble(vv);
                                        break;

                                    case "41":
                                        vertex.EndWidth = ToDouble(vv);
                                        break;

                                    case "42":
                                        vertex.Bulge = ToDouble(vv);
                                        break;
                                }

                                i += 2;
                            }

                            polyline.Vertices.Add(vertex);
                            continue;
                        }

                        i += 2;
                    }

                    if (polyline.Vertices.Count >= 2)
                    {
                        result.Add(polyline);
                    }

                    continue;
                }

                i += 2;
            }

            return result;
        }

        static int AddOldPolylinesToDwg(
            List<RawOldPolyline> polylines,
            CadDocument dwgDoc)
        {
            int count = 0;

            foreach (RawOldPolyline p in polylines)
            {
                LwPolyline lw = new LwPolyline();

                double defaultStartWidth = p.StartWidth;
                double defaultEndWidth = p.EndWidth;

                NormalizeWidthPair(ref defaultStartWidth, ref defaultEndWidth);

                bool polylineHeaderHasWidth =
                    Math.Abs(defaultStartWidth) > 0.0000001 ||
                    Math.Abs(defaultEndWidth) > 0.0000001;

                if (Math.Abs(defaultStartWidth - defaultEndWidth) < 0.0000001 &&
                    Math.Abs(defaultStartWidth) > 0.0000001)
                {
                    lw.ConstantWidth = defaultStartWidth;
                }

                bool hasAnyWidth = polylineHeaderHasWidth;

                foreach (RawOldPolylineVertex v in p.Vertices)
                {
                    double sw = v.StartWidth.HasValue ? v.StartWidth.Value : defaultStartWidth;
                    double ew = v.EndWidth.HasValue ? v.EndWidth.Value : defaultEndWidth;

                    NormalizeWidthPair(ref sw, ref ew);

                    if (Math.Abs(sw) > 0.0000001 ||
                        Math.Abs(ew) > 0.0000001)
                    {
                        hasAnyWidth = true;
                    }

                    AddLwPolylineVertex(
                        lw,
                        v.X,
                        v.Y,
                        sw,
                        ew,
                        v.Bulge);
                }

                SetPropertyIfExists(lw, "IsClosed", p.IsClosed);
                SetPropertyIfExists(lw, "Closed", p.IsClosed);

                SetPropertyIfExists(lw, "Elevation", p.Elevation);
                SetPropertyIfExists(lw, "Thickness", p.Thickness);

                Layer layer = FindLayer(dwgDoc, p.LayerName);
                if (layer != null)
                {
                    lw.Layer = layer;
                }

                double originalLineTypeScale =
                    Math.Abs(p.LineTypeScale) > 0.0000001 ? p.LineTypeScale : 1.0;

                // 方案二核心：
                // 实体 LineTypeScale 永远设置为 1；
                // 如果原来 48 不是 1，则把比例乘进线型定义。
                ApplyLineTypeWithScaledDefinition(
                    lw,
                    p.LineTypeName,
                    p.LayerName,
                    originalLineTypeScale,
                    dwgDoc);

                ApplyPlinegen(lw, p.Plinegen);

                if (hasAnyWidth)
                {
                    // 粗线强制绿色，ACI 3
                    SetEntityColour(lw);
                }
                else
                {
                    // 非粗线保留原颜色
                    ApplyColorIndex(lw, p.ColorIndex);
                }

                // 所有实体线型比例强制为 1
                lw.LineTypeScale = 1.0;

                dwgDoc.Entities.Add(lw);

                count++;
            }

            return count;
        }

        static void NormalizeWidthPair(ref double startWidth, ref double endWidth)
        {
            if (Math.Abs(startWidth) < 0.0000001 &&
                Math.Abs(endWidth) > 0.0000001)
            {
                startWidth = endWidth;
            }

            if (Math.Abs(endWidth) < 0.0000001 &&
                Math.Abs(startWidth) > 0.0000001)
            {
                endWidth = startWidth;
            }
        }

        static void AddLwPolylineVertex(
            LwPolyline polyline,
            double x,
            double y,
            double startWidth,
            double endWidth,
            double bulge)
        {
            object verticesObj = polyline.Vertices;

            if (verticesObj == null)
                throw new Exception("LwPolyline.Vertices 为空。");

            Type verticesType = verticesObj.GetType();

            Type vertexType = null;

            if (verticesType.IsGenericType)
            {
                vertexType = verticesType.GetGenericArguments().FirstOrDefault();
            }

            if (vertexType == null)
            {
                Type[] nestedTypes = typeof(LwPolyline).GetNestedTypes(
                    BindingFlags.Public | BindingFlags.NonPublic);

                vertexType = nestedTypes.FirstOrDefault(t =>
                    t.Name.IndexOf("Vertex", StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (vertexType == null)
            {
                throw new Exception("找不到 LwPolyline 顶点类型。");
            }

            object vertex = CreateVertexObject(vertexType, x, y);

            SetVertexXY(vertex, x, y);

            SetPropertyIfExists(vertex, "StartWidth", startWidth);
            SetPropertyIfExists(vertex, "EndWidth", endWidth);
            SetPropertyIfExists(vertex, "Bulge", bulge);

            MethodInfo addMethod = verticesType.GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "Add" &&
                    m.GetParameters().Length == 1);

            if (addMethod == null)
            {
                throw new Exception("找不到 LwPolyline.Vertices.Add 方法。");
            }

            addMethod.Invoke(verticesObj, new object[] { vertex });
        }

        static object CreateVertexObject(Type vertexType, double x, double y)
        {
            ConstructorInfo ctor2 = vertexType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    ParameterInfo[] ps = c.GetParameters();

                    return ps.Length == 2 &&
                           IsNumericType(ps[0].ParameterType) &&
                           IsNumericType(ps[1].ParameterType);
                });

            if (ctor2 != null)
            {
                ParameterInfo[] ps = ctor2.GetParameters();

                return ctor2.Invoke(new object[]
                {
                    Convert.ChangeType(x, ps[0].ParameterType, CultureInfo.InvariantCulture),
                    Convert.ChangeType(y, ps[1].ParameterType, CultureInfo.InvariantCulture)
                });
            }

            ConstructorInfo defaultCtor = vertexType.GetConstructor(Type.EmptyTypes);

            if (defaultCtor != null)
            {
                object vertex = defaultCtor.Invoke(null);
                SetVertexXY(vertex, x, y);
                return vertex;
            }

            throw new Exception("无法创建 LwPolyline 顶点对象。");
        }

        static void SetVertexXY(object vertex, double x, double y)
        {
            if (vertex == null)
                return;

            SetPropertyIfExists(vertex, "X", x);
            SetPropertyIfExists(vertex, "Y", y);

            TrySetPointProperty(vertex, "Location", x, y);
            TrySetPointProperty(vertex, "Position", x, y);
            TrySetPointProperty(vertex, "Point", x, y);
        }

        static void TrySetPointProperty(object obj, string propertyName, double x, double y)
        {
            if (obj == null)
                return;

            PropertyInfo prop = obj.GetType().GetProperty(propertyName);

            if (prop == null || !prop.CanWrite)
                return;

            try
            {
                object point = CreatePointObject(prop.PropertyType, x, y);

                if (point != null)
                {
                    prop.SetValue(obj, point);
                }
            }
            catch
            {
            }
        }

        static object CreatePointObject(Type pointType, double x, double y)
        {
            ConstructorInfo ctor2 = pointType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    ParameterInfo[] ps = c.GetParameters();

                    return ps.Length == 2 &&
                           IsNumericType(ps[0].ParameterType) &&
                           IsNumericType(ps[1].ParameterType);
                });

            if (ctor2 != null)
            {
                ParameterInfo[] ps = ctor2.GetParameters();

                return ctor2.Invoke(new object[]
                {
                    Convert.ChangeType(x, ps[0].ParameterType, CultureInfo.InvariantCulture),
                    Convert.ChangeType(y, ps[1].ParameterType, CultureInfo.InvariantCulture)
                });
            }

            ConstructorInfo ctor3 = pointType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    ParameterInfo[] ps = c.GetParameters();

                    return ps.Length == 3 &&
                           IsNumericType(ps[0].ParameterType) &&
                           IsNumericType(ps[1].ParameterType) &&
                           IsNumericType(ps[2].ParameterType);
                });

            if (ctor3 != null)
            {
                ParameterInfo[] ps = ctor3.GetParameters();

                return ctor3.Invoke(new object[]
                {
                    Convert.ChangeType(x, ps[0].ParameterType, CultureInfo.InvariantCulture),
                    Convert.ChangeType(y, ps[1].ParameterType, CultureInfo.InvariantCulture),
                    Convert.ChangeType(0.0, ps[2].ParameterType, CultureInfo.InvariantCulture)
                });
            }

            object point = Activator.CreateInstance(pointType);

            SetPropertyIfExists(point, "X", x);
            SetPropertyIfExists(point, "Y", y);
            SetPropertyIfExists(point, "Z", 0.0);

            return point;
        }

        static void FixNormalEntityDisplay(
            Entity srcEntity,
            Entity dstEntity,
            CadDocument targetDoc)
        {
            if (srcEntity.Layer != null)
            {
                Layer layer = FindLayer(targetDoc, srcEntity.Layer.Name);

                if (layer != null)
                {
                    dstEntity.Layer = layer;
                }
            }

            double originalLineTypeScale =
                Math.Abs(srcEntity.LineTypeScale) > 0.0000001 ? srcEntity.LineTypeScale : 1.0;

            string srcLineTypeName = null;
            string srcLayerName = null;

            if (srcEntity.LineType != null)
            {
                srcLineTypeName = srcEntity.LineType.Name;
            }

            if (srcEntity.Layer != null)
            {
                srcLayerName = srcEntity.Layer.Name;
            }

            // 普通实体也使用方案二：
            // 原实体线型比例不是 1 的时候，复制缩放线型；
            // 实体自身 LineTypeScale 设置为 1。
            ApplyLineTypeWithScaledDefinition(
                dstEntity,
                srcLineTypeName,
                srcLayerName,
                originalLineTypeScale,
                targetDoc);

            try
            {
                dstEntity.Color = srcEntity.Color;
            }
            catch
            {
            }

            try
            {
                dstEntity.LineWeight = srcEntity.LineWeight;
            }
            catch
            {
            }

            // 所有实体线型比例强制为 1
            dstEntity.LineTypeScale = 1.0;
        }

        /// <summary>
        /// 方案二核心方法。
        /// 如果原线型比例是 1，则直接用原线型。
        /// 如果原线型比例不是 1，则克隆原线型，并把线型定义里的段长乘以比例。
        /// 然后实体 LineTypeScale 仍然设置为 1。
        /// </summary>
        static void ApplyLineTypeWithScaledDefinition(
            Entity entity,
            string entityLineTypeName,
            string layerName,
            double originalEntityLineTypeScale,
            CadDocument dwgDoc)
        {
            if (entity == null)
                return;

            double scale =
                Math.Abs(originalEntityLineTypeScale) > 0.0000001
                    ? originalEntityLineTypeScale
                    : 1.0;

            string lineTypeName = entityLineTypeName;

            bool isByLayer =
                string.IsNullOrWhiteSpace(lineTypeName) ||
                lineTypeName.Equals("BYLAYER", StringComparison.OrdinalIgnoreCase);

            bool isByBlock =
                !string.IsNullOrWhiteSpace(lineTypeName) &&
                lineTypeName.Equals("BYBLOCK", StringComparison.OrdinalIgnoreCase);

            if (isByBlock)
            {
                entity.LineTypeScale = 1.0;
                return;
            }

            // 如果实体是 ByLayer，并且比例不是 1，为了保持显示效果，
            // 需要取图层线型，生成缩放后的线型，并显式赋给实体。
            if (isByLayer)
            {
                if (Math.Abs(scale - 1.0) < 0.0000001)
                {
                    entity.LineTypeScale = 1.0;
                    return;
                }

                Layer layer = FindLayer(dwgDoc, layerName);

                if (layer == null || layer.LineType == null)
                {
                    entity.LineTypeScale = 1.0;
                    return;
                }

                lineTypeName = layer.LineType.Name;
            }

            if (string.IsNullOrWhiteSpace(lineTypeName))
            {
                entity.LineTypeScale = 1.0;
                return;
            }

            if (lineTypeName.Equals("CONTINUOUS", StringComparison.OrdinalIgnoreCase))
            {
                LineType continuous = FindLineType(dwgDoc, lineTypeName);

                if (continuous != null)
                {
                    entity.LineType = continuous;
                }

                entity.LineTypeScale = 1.0;
                return;
            }

            LineType targetLineType;

            if (Math.Abs(scale - 1.0) < 0.0000001)
            {
                targetLineType = FindLineType(dwgDoc, lineTypeName);
            }
            else
            {
                targetLineType = GetOrCreateScaledLineType(
                    dwgDoc,
                    lineTypeName,
                    scale);
            }

            if (targetLineType != null)
            {
                entity.LineType = targetLineType;
            }

            entity.LineTypeScale = 1.0;
        }

        static LineType GetOrCreateScaledLineType(
            CadDocument doc,
            string baseLineTypeName,
            double scale)
        {
            if (doc == null)
                return null;

            if (string.IsNullOrWhiteSpace(baseLineTypeName))
                return null;

            if (Math.Abs(scale - 1.0) < 0.0000001)
                return FindLineType(doc, baseLineTypeName);

            LineType baseLineType = FindLineType(doc, baseLineTypeName);

            if (baseLineType == null)
                return null;

            string scaledName = BuildScaledLineTypeName(baseLineTypeName, scale);

            LineType existing = FindLineType(doc, scaledName);

            if (existing != null)
                return existing;

            try
            {
                LineType newLineType = (LineType)baseLineType.Clone();

                SetLineTypeName(newLineType, scaledName);

                TrySetStringProperty(
                    newLineType,
                    "Description",
                    GetStringProperty(baseLineType, "Description") +
                    " scaled " +
                    scale.ToString("0.########", CultureInfo.InvariantCulture));

                ScaleLineTypeDefinitionByReflection(newLineType, scale);

                doc.LineTypes.Add(newLineType);

                return newLineType;
            }
            catch
            {
                return baseLineType;
            }
        }

        static string BuildScaledLineTypeName(string baseName, double scale)
        {
            string scaleText = scale.ToString("0.########", CultureInfo.InvariantCulture);

            scaleText = scaleText
                .Replace("-", "M")
                .Replace("+", "")
                .Replace(".", "_");

            string safeBase = SanitizeName(baseName);

            string name = safeBase + "__S" + scaleText;

            if (name.Length > 240)
                name = name.Substring(0, 240);

            return name;
        }

        static string SanitizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "LT";

            StringBuilder sb = new StringBuilder();

            foreach (char ch in name)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-' || ch == '$')
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append('_');
                }
            }

            return sb.ToString();
        }

        static void SetLineTypeName(LineType lineType, string name)
        {
            if (lineType == null)
                return;

            SetPropertyIfExists(lineType, "Name", name);
        }

        /// <summary>
        /// 用反射缩放线型定义。
        /// 主要处理：
        /// PatternLength / Length / TotalLength
        /// Segments 里的 Length / DashLength / ElementLength / Offset / Scale / Height 等。
        /// </summary>
        static void ScaleLineTypeDefinitionByReflection(object lineType, double scale)
        {
            if (lineType == null)
                return;

            Type type = lineType.GetType();

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead)
                    continue;

                if (prop.GetIndexParameters().Length > 0)
                    continue;

                if (IsScalableLineTypeNumberProperty(prop.Name, prop.PropertyType))
                {
                    TryMultiplyNumericProperty(lineType, prop, scale);
                    continue;
                }

                if (prop.PropertyType == typeof(string))
                    continue;

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    object collection = null;

                    try
                    {
                        collection = prop.GetValue(lineType);
                    }
                    catch
                    {
                    }

                    if (collection == null)
                        continue;

                    ScaleEnumerableItems(collection, scale);
                }
            }
        }

        static void ScaleEnumerableItems(object collection, double scale)
        {
            if (collection == null)
                return;

            IList list = collection as IList;

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    object item = list[i];

                    if (item == null)
                        continue;

                    Type itemType = item.GetType();

                    if (IsNumericType(itemType))
                    {
                        try
                        {
                            double oldValue = Convert.ToDouble(item, CultureInfo.InvariantCulture);
                            double newValue = oldValue * scale;

                            list[i] = Convert.ChangeType(
                                newValue,
                                Nullable.GetUnderlyingType(itemType) ?? itemType,
                                CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        ScaleLineTypeSegmentObject(item, scale);
                    }
                }

                return;
            }

            IEnumerable enumerable = collection as IEnumerable;

            if (enumerable == null)
                return;

            foreach (object item in enumerable)
            {
                if (item == null)
                    continue;

                ScaleLineTypeSegmentObject(item, scale);
            }
        }

        static void ScaleLineTypeSegmentObject(object segment, double scale)
        {
            if (segment == null)
                return;

            Type type = segment.GetType();

            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (prop.GetIndexParameters().Length > 0)
                    continue;

                if (!IsScalableLineTypeSegmentNumberProperty(prop.Name, prop.PropertyType))
                    continue;

                TryMultiplyNumericProperty(segment, prop, scale);
            }
        }

        static bool IsScalableLineTypeNumberProperty(string propertyName, Type propertyType)
        {
            if (!IsNumericType(propertyType))
                return false;

            if (string.IsNullOrWhiteSpace(propertyName))
                return false;

            string n = propertyName.ToUpperInvariant();

            if (n.Contains("HANDLE"))
                return false;

            if (n.Contains("COLOR"))
                return false;

            if (n.Contains("INDEX"))
                return false;

            if (n.Contains("ANGLE"))
                return false;

            if (n.Contains("ROTATION"))
                return false;

            if (n == "LENGTH")
                return true;

            if (n.Contains("PATTERN") && n.Contains("LENGTH"))
                return true;

            if (n.Contains("TOTAL") && n.Contains("LENGTH"))
                return true;

            if (n.Contains("DASH") && n.Contains("LENGTH"))
                return true;

            return false;
        }

        static bool IsScalableLineTypeSegmentNumberProperty(string propertyName, Type propertyType)
        {
            if (!IsNumericType(propertyType))
                return false;

            if (string.IsNullOrWhiteSpace(propertyName))
                return false;

            string n = propertyName.ToUpperInvariant();

            if (n.Contains("HANDLE"))
                return false;

            if (n.Contains("COLOR"))
                return false;

            if (n.Contains("INDEX"))
                return false;

            if (n.Contains("ANGLE"))
                return false;

            if (n.Contains("ROTATION"))
                return false;

            if (n == "LENGTH")
                return true;

            if (n.Contains("LENGTH"))
                return true;

            if (n.Contains("OFFSET"))
                return true;

            if (n.Contains("SCALE"))
                return true;

            if (n.Contains("HEIGHT"))
                return true;

            return false;
        }

        static void TryMultiplyNumericProperty(object obj, PropertyInfo prop, double scale)
        {
            if (obj == null || prop == null)
                return;

            if (!prop.CanRead || !prop.CanWrite)
                return;

            try
            {
                object oldObj = prop.GetValue(obj);

                if (oldObj == null)
                    return;

                double oldValue = Convert.ToDouble(oldObj, CultureInfo.InvariantCulture);
                double newValue = oldValue * scale;

                Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                object newObj = Convert.ChangeType(
                    newValue,
                    targetType,
                    CultureInfo.InvariantCulture);

                prop.SetValue(obj, newObj);
            }
            catch
            {
            }
        }

        static void ApplyColorIndex(Entity entity, int? colorIndex)
        {
            if (entity == null)
                return;

            if (!colorIndex.HasValue)
                return;

            SetEntityColorByAci(entity, colorIndex.Value);
        }

        static void SetEntityColour(Entity entity)
        {
            if (entity == null)
                return;

            // AutoCAD ACI 3 = Green
            SetEntityColorByAci(entity, 2);
        }

        static void SetEntityColorByAci(Entity entity, int colorIndex)
        {
            if (entity == null)
                return;

            try
            {
                PropertyInfo colorProp = entity.GetType().GetProperty("Color");

                if (colorProp == null || !colorProp.CanWrite)
                    return;

                Type colorType = colorProp.PropertyType;

                object colorObj = CreateColorByAci(colorType, colorIndex);

                if (colorObj != null)
                {
                    colorProp.SetValue(entity, colorObj);
                }
            }
            catch
            {
            }
        }

        static object CreateColorByAci(Type colorType, int colorIndex)
        {
            if (colorType == null)
                return null;

            string[] staticMethodNames =
            {
                "FromCadIndex",
                "FromColorIndex",
                "FromIndex",
                "FromACI",
                "FromAci",
                "ByAci",
                "ByACI",
                "FromAutoCadColorIndex"
            };

            foreach (string methodName in staticMethodNames)
            {
                MethodInfo method = colorType.GetMethods(
                        BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase) &&
                        m.GetParameters().Length == 1);

                if (method == null)
                    continue;

                try
                {
                    ParameterInfo p = method.GetParameters()[0];

                    object arg = Convert.ChangeType(
                        colorIndex,
                        Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType,
                        CultureInfo.InvariantCulture);

                    object color = method.Invoke(null, new object[] { arg });

                    if (color != null)
                        return color;
                }
                catch
                {
                }
            }

            ConstructorInfo oneArgCtor = colorType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    ParameterInfo[] ps = c.GetParameters();

                    return ps.Length == 1 && IsNumericType(ps[0].ParameterType);
                });

            if (oneArgCtor != null)
            {
                try
                {
                    ParameterInfo p = oneArgCtor.GetParameters()[0];

                    object arg = Convert.ChangeType(
                        colorIndex,
                        Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType,
                        CultureInfo.InvariantCulture);

                    return oneArgCtor.Invoke(new object[] { arg });
                }
                catch
                {
                }
            }

            ConstructorInfo defaultCtor = colorType.GetConstructor(Type.EmptyTypes);

            if (defaultCtor != null)
            {
                try
                {
                    object color = defaultCtor.Invoke(null);

                    string[] indexPropertyNames =
                    {
                        "Index",
                        "ColorIndex",
                        "Aci",
                        "ACI",
                        "CadIndex",
                        "AutoCadColorIndex",
                        "AutoCADColorIndex"
                    };

                    foreach (string propName in indexPropertyNames)
                    {
                        PropertyInfo prop = colorType.GetProperty(propName);

                        if (prop == null || !prop.CanWrite)
                            continue;

                        try
                        {
                            object value = Convert.ChangeType(
                                colorIndex,
                                Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType,
                                CultureInfo.InvariantCulture);

                            prop.SetValue(color, value);

                            return color;
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }

            if (colorIndex == 3)
            {
                string[] greenPropertyNames =
                {
                    "Green",
                    "ByGreen"
                };

                foreach (string propName in greenPropertyNames)
                {
                    try
                    {
                        PropertyInfo greenProp = colorType.GetProperty(
                            propName,
                            BindingFlags.Public | BindingFlags.Static);

                        if (greenProp != null)
                        {
                            object green = greenProp.GetValue(null);

                            if (green != null)
                                return green;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return null;
        }

        static void ApplyPlinegen(LwPolyline lw, bool plinegen)
        {
            if (lw == null)
                return;

            string[] propNames =
            {
                "LinetypeGeneration",
                "LineTypeGeneration",
                "Plinegen",
                "PlineGen",
                "IsLinetypeGenerationEnabled"
            };

            foreach (string propName in propNames)
            {
                PropertyInfo prop = lw.GetType().GetProperty(propName);

                if (prop == null || !prop.CanWrite)
                    continue;

                try
                {
                    prop.SetValue(lw, plinegen);
                    return;
                }
                catch
                {
                }
            }
        }

        static void MergeLineTypes(CadDocument sourceDoc, CadDocument targetDoc)
        {
            foreach (LineType srcLineType in sourceDoc.LineTypes)
            {
                if (srcLineType == null)
                    continue;

                if (string.IsNullOrWhiteSpace(srcLineType.Name))
                    continue;

                if (FindLineType(targetDoc, srcLineType.Name) != null)
                    continue;

                try
                {
                    LineType newLineType = (LineType)srcLineType.Clone();
                    targetDoc.LineTypes.Add(newLineType);
                }
                catch
                {
                }
            }
        }

        static void MergeLayers(CadDocument sourceDoc, CadDocument targetDoc)
        {
            foreach (Layer srcLayer in sourceDoc.Layers)
            {
                if (srcLayer == null)
                    continue;

                if (string.IsNullOrWhiteSpace(srcLayer.Name))
                    continue;

                Layer dstLayer = FindLayer(targetDoc, srcLayer.Name);

                if (dstLayer == null)
                {
                    try
                    {
                        Layer newLayer = (Layer)srcLayer.Clone();

                        if (srcLayer.LineType != null)
                        {
                            LineType targetLineType =
                                FindLineType(targetDoc, srcLayer.LineType.Name);

                            if (targetLineType != null)
                            {
                                newLayer.LineType = targetLineType;
                            }
                        }

                        targetDoc.Layers.Add(newLayer);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        dstLayer.Color = srcLayer.Color;
                    }
                    catch
                    {
                    }

                    try
                    {
                        dstLayer.LineWeight = srcLayer.LineWeight;
                    }
                    catch
                    {
                    }

                    if (srcLayer.LineType != null)
                    {
                        LineType targetLineType =
                            FindLineType(targetDoc, srcLayer.LineType.Name);

                        if (targetLineType != null)
                        {
                            dstLayer.LineType = targetLineType;
                        }
                    }
                }
            }
        }

        static Layer FindLayer(CadDocument doc, string layerName)
        {
            if (doc == null)
                return null;

            if (string.IsNullOrWhiteSpace(layerName))
                return null;

            return doc.Layers.FirstOrDefault(x =>
                x != null &&
                string.Equals(x.Name, layerName, StringComparison.OrdinalIgnoreCase));
        }

        static LineType FindLineType(CadDocument doc, string lineTypeName)
        {
            if (doc == null)
                return null;

            if (string.IsNullOrWhiteSpace(lineTypeName))
                return null;

            return doc.LineTypes.FirstOrDefault(x =>
                x != null &&
                string.Equals(x.Name, lineTypeName, StringComparison.OrdinalIgnoreCase));
        }

        static string GetStringProperty(object obj, string propertyName)
        {
            if (obj == null)
                return "";

            try
            {
                PropertyInfo prop = obj.GetType().GetProperty(propertyName);

                if (prop == null || !prop.CanRead)
                    return "";

                object value = prop.GetValue(obj);

                return value == null ? "" : value.ToString();
            }
            catch
            {
                return "";
            }
        }

        static void TrySetStringProperty(object obj, string propertyName, string value)
        {
            SetPropertyIfExists(obj, propertyName, value);
        }

        static void SetPropertyIfExists(object obj, string propertyName, object value)
        {
            if (obj == null)
                return;

            PropertyInfo prop = obj.GetType().GetProperty(propertyName);

            if (prop == null || !prop.CanWrite)
                return;

            try
            {
                object convertedValue = value;

                if (value != null)
                {
                    Type targetType =
                        Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    if (targetType != value.GetType())
                    {
                        convertedValue = Convert.ChangeType(
                            value,
                            targetType,
                            CultureInfo.InvariantCulture);
                    }
                }

                prop.SetValue(obj, convertedValue);
            }
            catch
            {
            }
        }

        static bool IsNumericType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }

        static double ToDouble(string s)
        {
            double value;

            if (double.TryParse(
                s,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out value))
            {
                return value;
            }

            return 0.0;
        }
    }

    internal class RawOldPolyline
    {
        public string LayerName { get; set; }

        public string LineTypeName { get; set; }

        public int? ColorIndex { get; set; }

        public double StartWidth { get; set; }

        public double EndWidth { get; set; }

        public double LineTypeScale { get; set; }

        public int Flags { get; set; }

        public bool IsClosed { get; set; }

        public bool Plinegen { get; set; }

        public double Elevation { get; set; }

        public double Thickness { get; set; }

        public List<RawOldPolylineVertex> Vertices { get; } =
            new List<RawOldPolylineVertex>();
    }

    internal class RawOldPolylineVertex
    {
        public string LayerName { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double? StartWidth { get; set; }

        public double? EndWidth { get; set; }

        public double Bulge { get; set; }
    }
}
