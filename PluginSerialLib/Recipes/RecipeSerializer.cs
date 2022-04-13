using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;

namespace PluginSerialLib.Recipes
{

    public  class RecipeSerializer
    {

        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
        public RecipeSerializer()
        {
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            settings.Converters.Add(new RecipeJsonConverter());
        }


        public string RecipeToString(SerialPortRecipe recipe)
        {
        
            string Serialized = JsonConvert.SerializeObject(recipe, settings);
            return Serialized;
        }

        public  SerialPortRecipe RecipeFromString(string jsontext)
        {

            SerialPortRecipe recipeDeserial = JsonConvert.DeserializeObject<SerialPortRecipe>(jsontext, settings);
            return recipeDeserial;
        }

        public void RecipeToFile(SerialPortRecipe recipe, string path)
        {
            File.WriteAllText(path, RecipeToString(recipe));
        }
        public SerialPortRecipe RecipeFromFIle(string path)
        {
            string jsontext;
            using (StreamReader file = File.OpenText(path))
            {
                jsontext = file.ReadToEnd();
                return RecipeFromString(jsontext);
            }
        }
    }


    public class RecipeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(SerialPortRecipe).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            // Using a nullable bool here in case "is_album" is not present on an item
            string recipeType = (string)jo[nameof(SerialPortRecipe.RecipeType)];

            foreach (Type type in Assembly.GetAssembly(typeof(SerialPortRecipe)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SerialPortRecipe))))
            {
                var typeName = type.Name;
                if (typeName == recipeType)
                {
                    var rcp = (SerialPortRecipe)Activator.CreateInstance(type);
                    serializer.Populate(jo.CreateReader(), rcp);
                    return rcp;
                }
            }


            throw new JsonReaderException("Could not find a valid Recipe to instantiate");



        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            serializer.Serialize(writer, value);
        }
    }
}
