using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_test_payment_api.Context;
using tech_test_payment_api.Models;


namespace tech_test_payment_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly VendaContext _context;

        public PaymentController(VendaContext context)
        {
            _context = context;
        }



        [HttpPost("Registrar venda")]
        public IActionResult RegistrarVenda(Venda venda)
        {
             if (venda.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            if (venda.Itens == null)
                return BadRequest(new { Erro = "O campo Itens deve ser preenchido" });

            if (venda.Valor < 0)
                return BadRequest(new { Erro = "O Valor do item tem que ser maior que zero" });

            venda.Status = 0;
            _context.Add(venda);
            _context.SaveChanges();

            return CreatedAtAction(nameof(BuscarVenda), new {id = venda.Id}, venda);
        }


        [HttpGet("Buscar venda por ID")]
        public IActionResult BuscarVenda(int id)
        {
            var venda = _context.Vendas.Find(id);
            
            if (venda == null) { return NotFound(); }

            return Ok(venda);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            // TODO: Buscar todas as tarefas no banco utilizando o EF
            var vendas = _context.Vendas.ToList();
            return Ok(vendas);
        }

        /* [HttpPut("Atuilizar universal ")]
        public IActionResult AtualizarVenda(int id, Venda venda)
        {
            var vendaBanco = _context.Vendas.Find(id);

            if (vendaBanco == null)
                return NotFound();
            
            vendaBanco.Status = venda.Status;
                
            _context.Vendas.Update(vendaBanco);
            _context.SaveChanges();

            return Ok(vendaBanco);
        }*/

        [HttpPut("AtualizarVenda De: AguardandoPagamento para: PagamentoAprovado ou Cancelada De: PagamentoAprovado Para: EnviadoParaTransportadora ou Cancelada De: EnviadoParaTransportador. Para: Entregue")]
        public IActionResult AtualizarVenda(int id, EnumStatusVenda Status)
        {
            var vendaBanco = _context.Vendas.Find(id);
            if (vendaBanco == null)
            {
                return NotFound("Venda não encontrada");
            }

            Venda vendaAtualizada = new Venda
            {
                Id = id,
                Status = Status
            };

            switch (vendaBanco.Status)
            {
                case EnumStatusVenda.AguardandoPagamento:
                    if (Status == EnumStatusVenda.PagamentoAprovado || Status == EnumStatusVenda.Cancelada)
                    {
                        break;
                    }
                    else
                    {
                        return BadRequest("Transição de status não permitida");
                    }
                case EnumStatusVenda.PagamentoAprovado:
                    if (Status == EnumStatusVenda.EnviadoParaTransportadora || Status == EnumStatusVenda.Cancelada)
                    {
                        break;
                    }
                    else
                    {
                        return BadRequest("Transição de status não permitida");
                    }
                case EnumStatusVenda.EnviadoParaTransportadora:
                    if (Status == EnumStatusVenda.Entregue)
                    {
                        break;
                    }
                    else
                    {
                        return BadRequest("Transição de status não permitida");
                    }
                default:
                    return BadRequest("Transição de status não permitida");
            }


            vendaBanco.Status = vendaAtualizada.Status;


            _context.Update(vendaBanco).State = EntityState.Modified;

            _context.SaveChanges();

            return Ok(vendaBanco);
        }




    }
}