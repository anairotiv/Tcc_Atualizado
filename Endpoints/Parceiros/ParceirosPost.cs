﻿using TirandoAsRodinhas.Domain.Register;
using TirandoAsRodinhas.Infra.Data;

namespace TirandoAsRodinhas.Endpoints.Parceiros;

public class ParceirosPost
{
    public static string Template => "/cadastrar";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(ParceirosRequest parceirosRequest, ApplicationDbContext context)
    {
        
        var endereco = new Endereco {
            CEP = parceirosRequest.Cep,
            Logradouro = parceirosRequest.Logradouro,
            Numero = parceirosRequest.Numero,
            Complemento = parceirosRequest.Complemento,
            Bairro = parceirosRequest.Bairro,
            Cidade = parceirosRequest.Cidade,
            Estado = parceirosRequest.Estado,
        };

        /*var contato = new Contato {
            TelCelular = parceirosRequest.TelCelular,
            TelFixo = parceirosRequest.TelFixo,
            Email = parceirosRequest.Email, 
        }; */
        var contato = new Contato(parceirosRequest.TelCelular, parceirosRequest.TelFixo, parceirosRequest.Email);

        if (!contato.IsValid) {
            return Results.ValidationProblem(contato.Notifications.ConvertToProblemDetails());
        }



        var parceiro = new Parceiro 
        {
            EnderecoId  = endereco.Id,
            ContatoId = contato.Id,
            CreatedOn = DateTime.Now,
            EditedOn = DateTime.Now,
            EmpresaId = parceirosRequest.EmpresaID,
        };

        if (parceirosRequest.Tipo=="PESSOA FISICA") {
            var pessoaFisica = new PessoaFisica (parceirosRequest.CPF_CNPJ, parceirosRequest.Nome_Razao) 
            {
                 ParceiroId = parceiro.Id
            };
            if (!pessoaFisica.IsValid) 
                return Results.ValidationProblem(pessoaFisica.Notifications.ConvertToProblemDetails());

            await context.PessoasFisicas.AddAsync(pessoaFisica);
        }
        else if (parceirosRequest.Tipo=="PESSOA JURIDICA") {
            var pessoaJuridica = new PessoaJuridica {
                RazaoSocial = parceirosRequest.Nome_Razao,
                CNPJ = parceirosRequest.CPF_CNPJ,
                ParceiroId = parceiro.Id
            };
            context.PessoaJuridicas.Add(pessoaJuridica);
        }
        else 
        {
            return Results.NotFound("O tipo de Pessoa deve ser informado: PESSOA FISICA OU PESSOA JURIDICA");
        }

        await context.Enderecos.AddAsync(endereco);
        await context.Contatos.AddAsync(contato);
        await context.Parceiros.AddAsync(parceiro);
        await context.SaveChangesAsync();

        return Results.Created($"/cadastrar/{parceiro.Id}", parceiro.Id);
    }
}
