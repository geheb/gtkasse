using GtKasse.Ui.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Users;

public class EditUserInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [Display(Name = "Telefonnummer")]
    [PhoneField]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Passwort")]
    [PasswordLengthField]
    public string? Password { get; set; }

    [Display(Name = "Rollen")]
    [RequiredField]
    public bool[] Roles { get; set; } = new bool[11];

    [Display(Name = "Debitoren-Nr.")]
    [TextLengthField]
    public string? DebtorNumber { get; set; }

    [Display(Name = "Adress-Nr.")]
    [TextLengthField]
    public string? AddressNumber { get; set; }

    public void From(UserDto dto)
    {
        Name = dto.Name;
        Email = dto.Email;
        PhoneNumber = dto.PhoneNumber;

        if (dto.Roles != null)
        {
            if (dto.Roles.Any(r => r == Core.Models.Roles.Admin)) Roles[0] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.Treasurer)) Roles[1] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.Kitchen)) Roles[2] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.Member)) Roles[3] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.Interested)) Roles[4] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.TripManager)) Roles[5] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.Chairperson)) Roles[6] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.UserManager)) Roles[7] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.FleetManager)) Roles[8] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.BoatManager)) Roles[9] = true;
            if (dto.Roles.Any(r => r == Core.Models.Roles.HouseManager)) Roles[10] = true;
        }
        DebtorNumber = dto.DebtorNumber;
        AddressNumber = dto.AddressNumber;
    }

    public void To(UserDto dto)
    {
        dto.Name = Name;
        dto.Email = Email;
        dto.PhoneNumber = PhoneNumber;

        var roles = new List<string>();
        if (Roles[0]) roles.Add(Core.Models.Roles.Admin);
        if (Roles[1]) roles.Add(Core.Models.Roles.Treasurer);
        if (Roles[2]) roles.Add(Core.Models.Roles.Kitchen);
        if (Roles[3]) roles.Add(Core.Models.Roles.Member);
        if (Roles[4]) roles.Add(Core.Models.Roles.Interested);
        if (Roles[5]) roles.Add(Core.Models.Roles.TripManager);
        if (Roles[6]) roles.Add(Core.Models.Roles.Chairperson);
        if (Roles[7]) roles.Add(Core.Models.Roles.UserManager);
        if (Roles[8]) roles.Add(Core.Models.Roles.FleetManager);
        if (Roles[9]) roles.Add(Core.Models.Roles.BoatManager);
        if (Roles[10]) roles.Add(Core.Models.Roles.HouseManager);
        dto.Roles = roles.ToArray();

        dto.DebtorNumber = DebtorNumber;
        dto.AddressNumber = AddressNumber;
    }
}
