namespace TirandoAsRodinhas.Domain.Register;

public class Contato : Entity
{
    public string TelCelular { get; set; }
    public string TelFixo { get; set; }
    public string Email { get; set; }

    public Contato(string telCelular, string telFixo, string email)
    {
        var contract = new Contract<Contato>()
          .IsLowerOrEqualsThan(telCelular, 11, "TelCelular")
          .IsGreaterOrEqualsThan(telCelular, 11, "TelCelular")
          .IsLowerOrEqualsThan(telFixo, 10, "TelFixo")
          .IsGreaterOrEqualsThan(telFixo, 10, "TelFixo")
          .IsEmailOrEmpty(email, "Email");

        AddNotifications(contract);

        TelCelular = telCelular;
        TelFixo = telFixo;
        Email = email;

    }


}
