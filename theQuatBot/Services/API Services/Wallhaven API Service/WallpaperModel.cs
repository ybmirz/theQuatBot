
namespace TheQuatBot.Services
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class WallpaperModel
    {
        [JsonProperty("data")]
        public Datum[] Data { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("short_url")]
        public Uri ShortUrl { get; set; }

        [JsonProperty("views")]
        public long Views { get; set; }

        [JsonProperty("favorites")]
        public long Favorites { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("purity")]
        public Purity Purity { get; set; }

        [JsonProperty("category")]
        public Category Category { get; set; }

        [JsonProperty("dimension_x")]
        public long DimensionX { get; set; }

        [JsonProperty("dimension_y")]
        public long DimensionY { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("ratio")]
        public string Ratio { get; set; }

        [JsonProperty("file_size")]
        public long FileSize { get; set; }

        [JsonProperty("file_type")]
        public FileType FileType { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("colors")]
        public string[] Colors { get; set; }

        [JsonProperty("path")]
        public Uri Path { get; set; }

        [JsonProperty("thumbs")]
        public Thumbs Thumbs { get; set; }
    }

    public partial class Thumbs
    {
        [JsonProperty("large")]
        public Uri Large { get; set; }

        [JsonProperty("original")]
        public Uri Original { get; set; }

        [JsonProperty("small")]
        public Uri Small { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("current_page")]
        public long CurrentPage { get; set; }

        [JsonProperty("last_page")]
        public long LastPage { get; set; }

        [JsonProperty("per_page")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PerPage { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("query")]
        public object Query { get; set; }

        [JsonProperty("seed")]
        public object Seed { get; set; }
    }

    public enum Category { General, People };

    public enum FileType { ImageJpeg, ImagePng };

    public enum Purity { Nsfw };

    public partial class WallpaperModel
    {
        public static WallpaperModel FromJson(string json) => JsonConvert.DeserializeObject<WallpaperModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this WallpaperModel self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CategoryConverter.Singleton,
                FileTypeConverter.Singleton,
                PurityConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class CategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Category) || t == typeof(Category?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "general":
                    return Category.General;
                case "people":
                    return Category.People;
            }
            return Category.General;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Category)untypedValue;
            switch (value)
            {
                case Category.General:
                    serializer.Serialize(writer, "general");
                    return;
                case Category.People:
                    serializer.Serialize(writer, "people");
                    return;
            }
            throw new Exception("Cannot marshal type Category");
        }
        public static readonly CategoryConverter Singleton = new CategoryConverter();
    }

    internal class FileTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FileType) || t == typeof(FileType?);
        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "image/jpeg":
                    return FileType.ImageJpeg;
                case "image/png":
                    return FileType.ImagePng;
            }
            throw new Exception("Cannot unmarshal type FileType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (FileType)untypedValue;
            switch (value)
            {
                case FileType.ImageJpeg:
                    serializer.Serialize(writer, "image/jpeg");
                    return;
                case FileType.ImagePng:
                    serializer.Serialize(writer, "image/png");
                    return;
            }
            throw new Exception("Cannot marshal type FileType");
        }
        public static readonly FileTypeConverter Singleton = new FileTypeConverter();
    }

    internal class PurityConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Purity) || t == typeof(Purity?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "nsfw")
            {
                return Purity.Nsfw;
            }
            return Purity.Nsfw;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Purity)untypedValue;
            if (value == Purity.Nsfw)
            {
                serializer.Serialize(writer, "nsfw");
                return;
            }
            throw new Exception("Cannot marshal type Purity");
        }

        public static readonly PurityConverter Singleton = new PurityConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
