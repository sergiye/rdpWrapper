using System.IO;
using System.Web.Script.Serialization;

namespace sergiye.Common {

  internal static class SerializeHelper {

    internal static string ToJson(this object value) {
      return new JavaScriptSerializer().Serialize(value);
    }

    internal static T FromJson<T>(this string json) {
      return new JavaScriptSerializer().Deserialize<T>(json);
    }

    internal static void ToJsonFile(this object value, string filePath) {
      File.WriteAllText(filePath, value.ToJson());
    }

    internal static T ReadJsonFile<T>(string fileName) where T : class {
      T result = null;
      if (File.Exists(fileName))
        result = File.ReadAllText(fileName).FromJson<T>();
      return result;
    }
  }
}