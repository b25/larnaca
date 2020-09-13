using System.Collections.Generic;

namespace Larnaca.Blueprints
{
    public interface IConfigger
    {
        IDictionary<string, object> GetConfig();

        /// <summary>
        /// case is ignored, key is case insensitive
        /// </summary>
        void LoadConfig<T>(IEnumerable<KeyValuePair<string, T>> config);

        /// <summary>
        /// Try to find the config value and parse/cast to the required type
        /// In the following cases the default value will be returned:
        /// - key is null/empty
        /// - key missing
        /// - value could not be cast to string
        /// - value could not be converted from byte[] with the UTF8 encoding
        /// case is ignored, key is case insensitive
        /// </summary>
        string GetConfig(string key, string? defaultValue = null);
        /// <summary>
        /// Try to find the config value and parse/cast to the required type
        /// In the following cases the default value will be returned:
        /// - key is null/empty
        /// - key missing
        /// - value could not be cast to the expected type
        /// - value could not be paresed to a primitive expected type
        /// - string value could not be deserialized with newtonsoft json
        /// - byte[] value could not be deserialized with the default serializer
        /// case is ignored, key is case insensitive
        /// </summary>
        T GetConfig<T>(string key, T defaultValue = default);
        /// <summary>
        /// Validate that the config value existing and can be parsed/cast to the required type
        /// Will return Fail in the following cases
        /// - key is null/empty
        /// - key missing
        /// - value could not be cast to the expected type
        /// - value could not be paresed to a primitive expected type
        /// - string value could not be deserialized with newtonsoft json
        /// - byte[] value could not be deserialized with the default serializer
        /// case is ignored, key is case insensitive 
        /// </summary>
        OperationResult ValidateConfig<T>(string key);
        /// <summary>
        /// Try to find the config value and parse/cast to the required type
        /// In the following cases the default value will be returned with a failing status code and a descriptive error message
        /// - key is null/empty
        /// - key missing
        /// - value could not be cast to the expected type
        /// - value could not be paresed to a primitive expected type
        /// - string value could not be deserialized with newtonsoft json
        /// - byte[] value could not be deserialized with the default serializer
        /// case is ignored, key is case insensitive
        /// </summary>
        OperationResult<T> TryGetConfig<T>(string key, T defaultValue = default);
    }
}
