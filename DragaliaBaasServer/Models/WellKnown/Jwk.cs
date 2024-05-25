using Microsoft.IdentityModel.Tokens;

namespace DragaliaBaasServer.Models.WellKnown;

public class Jwk
{
    public string Alg { get; set; }
    public string Kty { get; set; }
    public string Use { get; set; }
    public IList<string>? X5c { get; set; }
    public string N { get; set; }
    public string E { get; set; }
    public string Kid { get; set; }
    public string X5t { get; set; }

    public Jwk(JsonWebKey key)
    {
        Alg = key.Alg;
        Kty = key.Kty;
        Use = key.Use;
        X5c = key.X5c;
        N = key.N;
        E = key.E;
        Kid = key.Kid;
        X5t = key.X5t;
    }
}