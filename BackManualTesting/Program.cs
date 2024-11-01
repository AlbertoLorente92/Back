using Back.Implementation;
using Back.Interfaces;
using Back.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;

class Program
{
    private static readonly HttpClient client = new();
    private static readonly ITextEncryptionService _textEncryptionService;
    private static readonly IKeyVaultService _keyVaultService;
    private static readonly IConfiguration _configuration;
    static Program()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("KV_ADDRESS", "https://backkv.vault.azure.net/"),
                new KeyValuePair<string, string?>("KV:ApiKey", "ApiKey"),
                new KeyValuePair<string, string?>("KV:AesSecretKey", "AesSecretKey"),
                new KeyValuePair<string, string?>("KV:AesIV", "AesIV"),
            ])
            .Build();

        _keyVaultService = new KeyVaultService(_configuration);
        _textEncryptionService = new AesEncryptionService(_configuration, _keyVaultService);
    }
    static async Task Main(string[] args)
    {
        var apiKey = _keyVaultService.GetSecret(_configuration["KV:ApiKey"] ?? string.Empty);
        client.BaseAddress = new Uri("https://back-b5hbewgdf3hhghhp.canadacentral-01.azurewebsites.net/");
        client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

        Console.WriteLine("---------------------------------------------------------------------");
        await GetCompanies();
        Console.WriteLine("---------------------------------------------------------------------");
        await GetUsers();
        Console.WriteLine("---------------------------------------------------------------------");
        await GetCompanyById();
        Console.WriteLine("---------------------------------------------------------------------");
        await GetUserById();
        Console.WriteLine("---------------------------------------------------------------------");
        await CreateCompany();
        Console.WriteLine("---------------------------------------------------------------------");
        await GetCompanies();
        Console.WriteLine("---------------------------------------------------------------------");
    }

    static async Task GetCompanies()
    {
        await GetAllEntity<CompanyEntity>("Company/GetCompanies", "Company");
    }

    static async Task GetUsers()
    {
        await GetAllEntity<UserEntity>("User/GetUsers", "User");
    }

    static async Task GetCompanyById()
    {
        var encryptedMsg = _textEncryptionService.Encrypt("1123");
        await GetEntityByParam<CompanyEntity>($"Company/GetCompanyById?companyIdEncrypted={WebUtility.UrlEncode(encryptedMsg)}");
    }

    static async Task GetUserById()
    {
        var encryptedMsg = _textEncryptionService.Encrypt("1");
        await GetEntityByParam<UserEntity>($"User/GetUserById?userIdEncrypted={WebUtility.UrlEncode(encryptedMsg)}");
    }

    static async Task CreateCompany()
    {
        var createCompanyRequest = new CreateCompanyRequest() { Name = "Takitos", Vat = "Vat123" };
        var encryptedMsg = _textEncryptionService.SerielizeAndEncrypt(createCompanyRequest);
        var response = await PostEntity<CompanyEntity>($"Company/CreateCompany?encryptedCompanyCreationRequest={WebUtility.UrlEncode(encryptedMsg)}");
        if (response != null) {
            Console.WriteLine(response.ToString());
        }
    }

    static async Task GetEntityByParam<T>(string endpoint) where T : class
    {
        var response = await client.GetAsync(endpoint);
        var data = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var entity = _textEncryptionService.DecryptAndDeserialize<T>(data);
            Console.WriteLine(entity == null ? "Null" : entity.ToString());
        }
        else
        {
            Console.WriteLine($"GET for '{endpoint}' failed with status code: {response.StatusCode}");
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(data);
                if (errorResponse != null) {
                    Console.WriteLine($"Code '{errorResponse.ErrorCode}'. Msg: {errorResponse.Message}");
                }
            }
            catch
            {
                Console.WriteLine($"Msg '{data}'");
            }
        }
    }

    static async Task GetAllEntity<T>(string endpoint, string entityName) where T : class
    {
        var response = await client.GetAsync(endpoint);
        var data = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var entities = _textEncryptionService.DecryptAndDeserialize<List<T>>(data)!;
            Console.WriteLine($"{entityName} count is {entities.Count}.");
            for (int i = 0; i < entities.Count; i++)
            {
                Console.WriteLine($"{entityName} {i + 1}:");
                Console.WriteLine(entities[i].ToString());
            }
        }
        else
        {
            Console.WriteLine($"GET for '{endpoint}' failed with status code: {response.StatusCode}");
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(data);
                if (errorResponse != null)
                {
                    Console.WriteLine($"Code '{errorResponse.ErrorCode}'. Msg: {errorResponse.Message}");
                }
            }
            catch
            {
                Console.WriteLine($"Msg '{data}'");
            }
        }
    }

    static async Task<T?> PostEntity<T>(string endpoint) where T : class
    {
        var response = await client.PostAsync(endpoint, null);
        var data = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return _textEncryptionService.DecryptAndDeserialize<T>(data);
        }
        else
        {
            Console.WriteLine($"POST for '{endpoint}' failed with status code: {response.StatusCode}");
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(data);
                if (errorResponse != null)
                {
                    Console.WriteLine($"Code '{errorResponse.ErrorCode}'. Msg: {errorResponse.Message}");
                }
            }
            catch
            {
                Console.WriteLine($"Msg '{data}'");
            }
            return null;
        }
    }
}
