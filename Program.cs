// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;

namespace AttributeJson
{
    
    public partial class Program
    {
        enum ItemType
        {
            Array = 1,
            Object= 2
        }
        static Dictionary<int, string> dic1 = new();
        static Dictionary<int, List<string>> Values  = new();
        static void Scannode(JToken token, JToken parent, Action<JObject, JToken, string> scanAttributes, string searchItem)
        {
            if (token.Type == JTokenType.Object)
            {
                scanAttributes((JObject)token, parent, searchItem);

                foreach (JProperty child in token.Children<JProperty>())
                {
                    Scannode(child.Value, token, scanAttributes, searchItem);
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (JToken obj in token.Children())
                {

                    Scannode(obj, token, scanAttributes, searchItem);
                }
            }
        }

        public static string ReadFile(string filePath)
        {
            string json = null;
            using (StreamReader r = new StreamReader(filePath))
            {
                json = r.ReadToEnd();
            }
            return json;
        }
        static void Main(string[] args)
        {
            string json = ReadFile("value.json");
            JObject jsonDocument = JObject.Parse(json);
            string val = "custom_fields";
            
            Scannode(jsonDocument, jsonDocument, (node,parent,val) =>
            {
                JToken token = node[val];
                if (token != null)//existe el atributo
                {

                    if(token.Type == JTokenType.Array)
                    {
                        int arrId = (int)ItemType.Array;
                        if (Values.TryGetValue(arrId, out var listaArreglo))
                        {
                            listaArreglo.Add(token.ToString());//o directamente hacer parse 
                            Values[arrId] = listaArreglo;
                        }
                        else
                        {
                            var lista = new List<string>();
                            lista.Add(token.ToString());
                            Values.TryAdd(arrId, lista);
                        }
                    }
                    else if( token.Type ==JTokenType.Object)
                    {
                        var objId = (int)ItemType.Object;
                        if (Values.TryGetValue(objId, out var listaObjeto))
                        {
                            listaObjeto.Add(token.ToString());//o directamente hacer parse 
                            Values[objId] = listaObjeto;
                        }
                        else
                        {
                            var lista = new List<string>();
                            lista.Add(token.ToString());//o directamente hacer parse
                            Values.TryAdd(objId, lista);
                        }
                    }
                    var items= node.DescendantsAndSelf()
                   .OfType<JProperty>()
                   .Where(jp => jp.Value is JValue)
                   .Select(jp => jp.Path)
                   .Where(jp=>jp.Contains($".{val}"))
                   .ToList();

                    if(items.Any())
                    dic1.Add(dic1.Count + 1, items.First());
                }
            }, val);

            Console.WriteLine($"Path with Attribute {val}:\n" +
                $"{string.Join("\n",dic1.Values.Select(x=>x))}");
          
        }

    }
}



