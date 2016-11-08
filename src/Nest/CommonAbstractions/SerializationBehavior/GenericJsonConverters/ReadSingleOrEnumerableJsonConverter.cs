using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Nest
{
	internal class ReadSingleOrEnumerableJsonConverter<T> : JsonConverter
	{
		public override bool CanConvert(Type objectType) => true;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return reader.TokenType == JsonToken.StartArray
				? serializer.Deserialize<T[]>(reader)
				: new[] { serializer.Deserialize<T>(reader) };
		}

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}
	}

	// TODO: Evaluate if this is needed
	//internal class ReadAndWriteSingleOrEnumerableJsonConverter<T> : ReadSingleOrEnumerableJsonConverter<T>
	//{
	//	public override bool CanWrite => true;

	//	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	//	{
	//		var values = value as IEnumerable<T>;
	//		if (values == null)
	//		{
	//			writer.WriteNull();
	//			return;
	//		}

	//		if (values.Count() == 1) serializer.Serialize(writer, values.ElementAt(0));
	//		else serializer.Serialize(writer, values);
	//	}
	//}
}
