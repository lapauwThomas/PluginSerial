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

    public class RecipeSerializer
    {

        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
        public RecipeSerializer()
        {
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            settings.Converters.Add(new RecipeJsonConverter());
        }


        public string RecipeToString(RecipeBase recipe)
        {
        
            string Serialized = JsonConvert.SerializeObject(recipe, settings);
            return Serialized;
        }

        public  RecipeBase RecipeFromString(string jsontext, string name)
        {

            RecipeBase recipeDeserial = JsonConvert.DeserializeObject<RecipeBase>(jsontext, settings);
            recipeDeserial.Name = name;
            return recipeDeserial;
        }

        public void RecipeToFile(RecipeBase recipe, string path, string OverrideFilename = null)
        {
            string filename = recipe.Name;
            if (OverrideFilename != null)
            {
                filename = OverrideFilename;
            } 
            string filepath = Path.Combine(path, Path.ChangeExtension(filename, ConfigManager.DefaultRecipeExtension));
            File.WriteAllText(filepath, RecipeToString(recipe));
        }
        public RecipeBase RecipeFromFile(string path)
        {
            string jsontext;
            using (StreamReader file = File.OpenText(path))
            {
                jsontext = file.ReadToEnd();
                string name = Path.GetFileNameWithoutExtension(path);
                return RecipeFromString(jsontext, name);
            }
        }
    }


    public class RecipeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(RecipeBase).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            // Using a nullable bool here in case "is_album" is not present on an item
            string recipeType = (string)jo[nameof(RecipeBase.RecipeType)];

            foreach (Type type in Assembly.GetAssembly(typeof(RecipeBase)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(RecipeBase))))
            {
                var typeName = type.Name;
                if (typeName == recipeType)
                {
                    var rcp = (RecipeBase)Activator.CreateInstance(type);
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
