using System.ComponentModel.DataAnnotations;

namespace LpProjetoAtlantica.Models;

public class Contato
{

    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    public string Nome { get; set; }
    [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O endereço de e-mail fornecido não é válido.")]
    public string Email { get; set; }
}
