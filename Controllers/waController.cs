using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WaMVC.Data;
using WaMVC.Models;

namespace WaMVC.Controllers
{
    [Authorize]
    public class waController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<waController> _logger;
        private readonly ApplicationDbContext _database;

        public waController(ILogger<waController> logger, ApplicationDbContext database, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _database = database;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (_database.GFTs.Count() == 0) {
                GFT GftCuritiba = new GFT {Cep = "80250-210", Cidade = "Curitiba", Endereco = "Av. Sete de Setembro", Estado = "PR", Nome = "GFT Curitiba", Telefone = "(41) 3343-3601"};
                GFT GftAlphaville = new GFT {Cep = "06454-000", Cidade = "Barueri", Endereco = "Alphaville Industrial", Estado = "SP", Nome = "GFT Alphaville", Telefone = "(11) 2176-3253"};
                GFT GftSorocaba = new GFT {Cep = "02038-030", Cidade = "Sorocaba", Endereco = "Av. São Francisco", Estado = "SP", Nome = "GFT Sorocaba", Telefone = "(11) 97381-3068"};

                _database.GFTs.Add(GftCuritiba);
                _database.GFTs.Add(GftAlphaville);
                _database.GFTs.Add(GftSorocaba);

                _database.SaveChanges();
            }

            return View();
        }

        public IActionResult funcionarios(string Pesquisa = "")
        {
            var funcionario = _database.FuncionarioTecnologias.Where(p => p.Status == 0).Include(p => p.Funcionario).Include(p => p.Tecnologia).AsQueryable();

            if (_database.Funcionarios.FirstOrDefault() == null) {
                return RedirectToAction("notFuncionario");
            }
            
            if (!string.IsNullOrEmpty(Pesquisa)) {
                funcionario = funcionario.Where(p => p.Funcionario.Nome.Contains(Pesquisa));
            }

            funcionario = funcionario.OrderBy(p => p.Funcionario.Nome);

            return View(funcionario.ToList());
        }

        [Authorize(Policy = "haveName")]
        public IActionResult cadastrarFuncionario(GFT gft)
        {
            ViewBag.gftId = gft;
            return View();
        }
        
        [Authorize(Policy = "haveName")]
        [HttpPost]
        public IActionResult salvarFuncionario(FuncionarioTecnologia funcionarioTecnologia)
        {
            if (funcionarioTecnologia.Id == 0)
            {
                if(funcionarioTecnologia.Funcionario.LocalDeTrabalho.Nome == "GFT Curitiba") {
                    funcionarioTecnologia.Funcionario.LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Curitiba");
                }
                else if (funcionarioTecnologia.Funcionario.LocalDeTrabalho.Nome == "GFT Alphaville") {
                    funcionarioTecnologia.Funcionario.LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Alphaville");
                }
                else {
                    funcionarioTecnologia.Funcionario.LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Sorocaba");
                }
                DateTime dataFinal = funcionarioTecnologia.Funcionario.InicioWa.AddDays(15);
                funcionarioTecnologia.Funcionario.TerminoWa = dataFinal;
                _database.FuncionarioTecnologias.Add(funcionarioTecnologia);
            }
            else
            {
                FuncionarioTecnologia funcionarioTecnologiaBanco = _database.FuncionarioTecnologias.Include(p => p.Funcionario).Include(x => x.Tecnologia).First(p => p.Id == funcionarioTecnologia.Id);
                funcionarioTecnologiaBanco.Funcionario.Nome = funcionarioTecnologia.Funcionario.Nome;
                funcionarioTecnologiaBanco.Funcionario.Cargo = funcionarioTecnologia.Funcionario.Cargo;

                funcionarioTecnologiaBanco.Tecnologia.Nome = funcionarioTecnologia.Tecnologia.Nome;

                funcionarioTecnologiaBanco.Funcionario.InicioWa = funcionarioTecnologia.Funcionario.InicioWa;
                DateTime datafinal = funcionarioTecnologiaBanco.Funcionario.InicioWa.AddDays(15);
                funcionarioTecnologiaBanco.Funcionario.TerminoWa = datafinal;
                funcionarioTecnologiaBanco.Funcionario.Matricula = funcionarioTecnologia.Funcionario.Matricula;

                if (funcionarioTecnologia.Funcionario.LocalDeTrabalho.Nome == "GFT Curitiba") {
                    funcionarioTecnologiaBanco.Funcionario.LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Curitiba");
                }
                else if (funcionarioTecnologia.Funcionario.LocalDeTrabalho.Nome == "GFT Alphaville") {
                    funcionarioTecnologiaBanco.Funcionario.LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Alphaville");
                }
                else {
                    funcionarioTecnologiaBanco.Funcionario.LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Sorocaba");
                }
            }

            _database.SaveChanges();

            return RedirectToAction("funcionarios");
        }

        [Authorize(Policy = "haveName")]
        public IActionResult editarFuncionario(int id)
        {
            FuncionarioTecnologia funcionarioTecnologia = _database.FuncionarioTecnologias.Include(p => p.Funcionario).Include(x => x.Tecnologia)
                                                                                                                        .First(options => options.Id == id);

            return View("editarFuncionario", funcionarioTecnologia);
        }

        [Authorize(Policy = "haveName")]
        public IActionResult deletarFuncionario(int id)
        {
            FuncionarioTecnologia funcionarioTecnologia = _database.FuncionarioTecnologias.Include(p => p.Funcionario).Include(p => p.Tecnologia).First(options => options.Id == id);
            Funcionario funcionario = _database.Funcionarios.FirstOrDefault(p => p.Id == id);

            _database.FuncionarioTecnologias.Remove(funcionarioTecnologia);
            _database.Funcionarios.Remove(funcionario);

            _database.SaveChanges();

            if (funcionarioTecnologia.Equals(null)) {
                return RedirectToAction("Index");
            }
            else {
                return RedirectToAction("funcionarios");    
            }
        }

        [Authorize(Policy = "haveName")]
        public IActionResult vagas(string Pesquisa = "")
        {
            var vaga = _database.VagaTecnologias.Where(p => p.Status == 0).Include(p => p.Tecnologia).Include(x => x.Vaga).AsQueryable();
            
            if (_database.Vagas.FirstOrDefault() == null) {
                return RedirectToAction("notVaga");
            }

            if (!string.IsNullOrEmpty(Pesquisa)) {
                vaga = vaga.Where(p => p.Vaga.Projeto.Contains(Pesquisa));
            }

            vaga = vaga.OrderBy(p => p.Vaga.Projeto);

            return View(vaga.ToList());
        }

        [Authorize(Policy = "haveName")]
        public IActionResult cadastrarVaga()
        {
            return View();
        }

        [Authorize(Policy = "haveName")]
        [HttpPost]
        public IActionResult salvarVaga(VagaTecnologia vagaTecnologia)
        {
            if (vagaTecnologia.Id == 0)
            {
                _database.VagaTecnologias.Add(vagaTecnologia);
            }
            else
            {
                VagaTecnologia vagaTecnologiaBanco = _database.VagaTecnologias.Include(p => p.Vaga).Include(x => x.Tecnologia).FirstOrDefault(p => p.Id == vagaTecnologia.Id);
                vagaTecnologiaBanco.Vaga.AberturaVaga = vagaTecnologia.Vaga.AberturaVaga;
                vagaTecnologiaBanco.Vaga.CodigoVaga = vagaTecnologia.Vaga.CodigoVaga;
                vagaTecnologiaBanco.Vaga.DescricaoVaga = vagaTecnologia.Vaga.DescricaoVaga;
                vagaTecnologiaBanco.Vaga.Projeto = vagaTecnologia.Vaga.Projeto;
                vagaTecnologiaBanco.Vaga.QuantidadeVaga = vagaTecnologia.Vaga.QuantidadeVaga;

                vagaTecnologiaBanco.Tecnologia.Nome = vagaTecnologia.Tecnologia.Nome;
            }

            _database.SaveChanges();

            return RedirectToAction("vagas");
        }

        [Authorize(Policy = "haveName")]
        public IActionResult editarVaga(int id)
        {
             VagaTecnologia vagaTecnologia = _database.VagaTecnologias.Include(p => p.Vaga).Include(x => x.Tecnologia).First(options => options.Id == id);

            return View("editarVaga", vagaTecnologia);
        }

        [Authorize(Policy = "haveName")]
        public IActionResult deletarVaga(int id)
        {
             VagaTecnologia vagaTecnologia = _database.VagaTecnologias.Include(p => p.Vaga).Include(p => p.Tecnologia).FirstOrDefault(options => options.Id == id);
             Vaga vaga = _database.Vagas.FirstOrDefault(options => options.Id == id);

             if (_database.Aloc.FirstOrDefault(p => p.vag.Id == id) == null) {
                 _database.VagaTecnologias.Remove(vagaTecnologia);
                 _database.Vagas.Remove(vaga);

                _database.SaveChanges();
             }
             else {
                 return RedirectToAction("notDeletarVaga");
             }

            if (vagaTecnologia.Equals(null)) {
                return RedirectToAction("Index");
            }
            else {
                return RedirectToAction("vagas");
            }
        }
        [Authorize(Policy = "haveName")]
        public IActionResult listaFuncionarioAlocar()
        {
            var funcionario = _database.FuncionarioTecnologias.Where(p => p.Status == 0).Include(p => p.Funcionario).Include(x => x.Tecnologia).ToList();

            if (funcionario.Count == 0) {
                return RedirectToAction("notAlocar");
            }
            funcionario = funcionario.OrderBy(p => p.Funcionario.Nome).ToList();
            return View(funcionario);
        }

        [Authorize(Policy = "haveName")]
        public IActionResult alocarFuncionario(int id)
        {
            return RedirectToAction("listaVagaAlocar", new { id });
        }

        [Authorize(Policy = "haveName")]
        public IActionResult listaVagaAlocar(int id)
        {
            var vaga = _database.VagaTecnologias.Where(p => p.Status == 0).Include(p => p.Vaga).Include(x => x.Tecnologia).ToList();
            if (vaga.Count == 0) {
                return RedirectToAction("notVaga");
            }

            ViewBag.FuncionarioId = id;
            vaga = vaga.OrderBy(p => p.Vaga.Projeto).ToList();
            return View(vaga);
        }

        [Authorize(Policy = "haveName")]
        public IActionResult alocarVaga(int vagaId, int funcionarioId)
        {
            
            var funcionarios = _database.FuncionarioTecnologias.Include(p => p.Funcionario).Include(p => p.Tecnologia).FirstOrDefault(p => p.Id == funcionarioId);
            var vagas = _database.VagaTecnologias.Include(p => p.Vaga).Include(p => p.Tecnologia).FirstOrDefault(p => p.Id == vagaId);

            funcionarios.Status = 1;
            funcionarios.Funcionario.Alocacao = vagas.Vaga;
            vagas.Vaga.QuantidadeVaga -= 1;

            if (vagas.Vaga.QuantidadeVaga <= 0) {
                vagas.Status = 1;
            }
            else {
                vagas.Status = 0;
            }

            var funcionario = new Funcionario(funcionarios.Funcionario);
            var vaga = new Vaga(vagas.Vaga);

            var inicioAlocacao = DateTime.Now;

            Alocar alocar = new Alocar(funcionarios, vagas, inicioAlocacao);

            _database.Aloc.Add(alocar);
            _database.SaveChanges();

            _database.SaveChanges();

            return RedirectToAction("historico");
        }

        [Authorize(Policy = "haveName")]
        public IActionResult historico(string Pesquisa = "")
        {
            var puxarHistorico = _database.Aloc.Include(p => p.func).Include(p => p.vag).Include(p => p.func.Funcionario).Include(p => p.func.Tecnologia).Include(p => p.vag.Vaga).AsQueryable();

            if (_database.Aloc.FirstOrDefault() == null) {
                return RedirectToAction("notHistorico");
            }

            if (!string.IsNullOrEmpty(Pesquisa)) {
                puxarHistorico = puxarHistorico.Where(p => p.func.Funcionario.Nome.Contains(Pesquisa));
            }

            puxarHistorico = puxarHistorico.OrderBy(p => p.func.Funcionario.Nome);

            return View(puxarHistorico.ToList());
        }

        [Authorize]
        public IActionResult PopularBase()
        {
            foreach (var funcionario in _database.Funcionarios)
            {
                if (funcionario.Nome == "Vergara")
                    return RedirectToAction("PopularExist");
            }

            DateTime data1 = DateTime.Parse("2020-10-20 15:47:35");
            DateTime data2 = DateTime.Parse("2020-10-15 10:30:15");
            DateTime data3 = DateTime.Parse("2020-10-18 15:20:40");
            DateTime data4 = DateTime.Parse("2020-09-10 13:50:12");
            DateTime data5 = DateTime.Parse("2020-10-26 16:45:10");

            // VAGA
            Vaga vagaDotnetJr = new Vaga {AberturaVaga = data1, CodigoVaga = "7", DescricaoVaga = "Desenvolvedor .NET Júnior", Projeto = "UBS Bank", QuantidadeVaga = 15};
            Vaga vagaDotnetSr = new Vaga {AberturaVaga = data2, CodigoVaga = "5", DescricaoVaga = "Desenvolvedor .NET Sênior", Projeto = "UBS Bank", QuantidadeVaga = 10};
            Vaga vagaJavaSr = new Vaga {AberturaVaga = data3, CodigoVaga = "13", DescricaoVaga = "Desenvolvedor Java Sênior", Projeto = "Banco Original", QuantidadeVaga = 20};
            Vaga vagaAzure = new Vaga {AberturaVaga = data4, CodigoVaga = "8", DescricaoVaga = "Desenvolvedor Azure", Projeto = "Deutsche Bank", QuantidadeVaga = 3};
            Vaga vagaAws = new Vaga {AberturaVaga = data5, CodigoVaga = "2", DescricaoVaga = "Desenvolvedor AWS", Projeto = "Santander", QuantidadeVaga = 6};

            // TECNOLOGIA
            Tecnologia dotnet = new Tecnologia{Nome = ".NET"};
            Tecnologia java = new Tecnologia{Nome = "Java"};
            Tecnologia Azure = new Tecnologia{Nome = "Azure"};
            Tecnologia AWS = new Tecnologia{Nome = "AWS"};

            //VAGA TECNOLOGIA
            VagaTecnologia vagaTec1 = new VagaTecnologia {Vaga = vagaDotnetJr, Tecnologia = dotnet};
            VagaTecnologia vagaTec2 = new VagaTecnologia {Vaga = vagaDotnetSr, Tecnologia = dotnet};
            VagaTecnologia vagaTec3 = new VagaTecnologia {Vaga = vagaJavaSr, Tecnologia = java};
            VagaTecnologia vagaTec4 = new VagaTecnologia {Vaga = vagaAzure, Tecnologia = Azure};
            VagaTecnologia vagaTec5 = new VagaTecnologia {Vaga = vagaAws, Tecnologia = AWS};

            //FUNCIONARIO
            DateTime datainicio = DateTime.Now;
            DateTime datafinal = datainicio.AddDays(15);

            Funcionario funcionario1 = new Funcionario {Nome = "Eduardo",
                                                        Cargo = "Desenvolvedor .NET",
                                                        InicioWa = datainicio, TerminoWa =  datafinal,
                                                        Matricula = "#1201",
                                                        LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Curitiba"),
                                                        };

            Funcionario funcionario2 = new Funcionario {Nome = "Clécio",
                                                        Cargo = "Desenvolvedor .NET",
                                                        InicioWa = datainicio, TerminoWa =  datafinal,
                                                        Matricula = "#1560",
                                                        LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Alphaville"),
                                                        };

            Funcionario funcionario3 = new Funcionario {Nome = "Henrique",
                                                        Cargo = "Desenvolvedor Java",
                                                        InicioWa = datainicio, TerminoWa =  datafinal,
                                                        Matricula = "#5601",
                                                        LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Sorocaba"),
                                                        };

            Funcionario funcionario4 = new Funcionario {Nome = "Vidal",
                                                        Cargo = "Desenvolvedor Azure",
                                                        InicioWa = datainicio, TerminoWa =  datafinal,
                                                        Matricula = "#4862",
                                                        LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Sorocaba"),
                                                        };

            Funcionario funcionario5 = new Funcionario {Nome = "Vergara",
                                                        Cargo = "Desenvolvedor AWS",
                                                        InicioWa = datainicio, TerminoWa =  datafinal,
                                                        Matricula = "#7562",
                                                        LocalDeTrabalho = _database.GFTs.FirstOrDefault(p => p.Nome == "GFT Alphaville"),
                                                        };

            FuncionarioTecnologia funcionarioTec1 = new FuncionarioTecnologia {Funcionario = funcionario1, Tecnologia = dotnet};
            FuncionarioTecnologia funcionarioTec2 = new FuncionarioTecnologia {Funcionario = funcionario2, Tecnologia = dotnet};
            FuncionarioTecnologia funcionarioTec3 = new FuncionarioTecnologia {Funcionario = funcionario3, Tecnologia = java};
            FuncionarioTecnologia funcionarioTec4 = new FuncionarioTecnologia {Funcionario = funcionario4, Tecnologia = Azure};
            FuncionarioTecnologia funcionarioTec5 = new FuncionarioTecnologia {Funcionario = funcionario5, Tecnologia = AWS};
        
            _database.Add(vagaDotnetJr);
            _database.Add(vagaDotnetSr);
            _database.Add(vagaJavaSr);
            _database.Add(vagaAzure);
            _database.Add(vagaAws);

            _database.Add(dotnet);
            _database.Add(java);
            _database.Add(Azure);
            _database.Add(AWS);

            _database.Add(vagaTec1);
            _database.Add(vagaTec2);
            _database.Add(vagaTec3);
            _database.Add(vagaTec4);
            _database.Add(vagaTec5);

            _database.Add(funcionario1);
            _database.Add(funcionario2);
            _database.Add(funcionario3);
            _database.Add(funcionario4);
            _database.Add(funcionario5);

            _database.Add(funcionarioTec1);
            _database.Add(funcionarioTec2);
            _database.Add(funcionarioTec3);
            _database.Add(funcionarioTec4);
            _database.Add(funcionarioTec5);

            _database.SaveChanges();

            return View();
        }

        public IActionResult PopularExist()
        {
            return View();
        }

        public IActionResult notFuncionario()
        {
            return View();
        }

        public IActionResult notVaga()
        {
            return View();
        }

        public IActionResult notAlocar()
        {
            return View();
        }

        public IActionResult notHistorico()
        {
            return View();
        }

        public IActionResult notDeletarVaga()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}