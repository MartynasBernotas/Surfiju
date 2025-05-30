using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GoogleLocationService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "AIzaSyAAVP_llxal1IGKKGkVX76V-IIx8Vur2F8";

    public GoogleLocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<LocationSearchResponse>> GetPlaceSuggestionsAsync(string input)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={input}&key={ApiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var autocompleteResponse = JsonSerializer.Deserialize<PlaceAutocompleteResponse>(json);

      return autocompleteResponse is null ? new List<LocationSearchResponse>() :
            autocompleteResponse.Predictions
            .Select(p => new LocationSearchResponse(p.StructuredFormatting.MainText, p.StructuredFormatting.SecondaryText))
            .ToList();
    }

    public async Task<string> GetCoordinatesAsync(string address)
    {
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={ApiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return json; // Parse and return coordinates as needed
    }
}

public class PlaceAutocompleteResponse
{
    [JsonPropertyName("predictions")]
    public List<Prediction> Predictions { get; set; }
}

public class Prediction
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("structured_formatting")]
    public StructuredFormatting StructuredFormatting { get; set; }
}

public class StructuredFormatting
{
    [JsonPropertyName("main_text")]
    public string MainText { get; set; } // City
    [JsonPropertyName("secondary_text")]
    public string SecondaryText { get; set; } // Country
}

public record LocationSearchResponse(string City, string Country);
