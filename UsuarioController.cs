using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webescola.DAO;
using PagedList;
using Webescola.Models;
using Webescola.Controllers;
using System.Collections;


namespace WebBoleto.Controllers
{
    
    public class UsuarioController : Controller
    {
        string Login;

  
        public ActionResult Index(int? pagina)
        {
            int paginatamanho = 5;
            int paginanumero = (pagina ?? 1);

            UsuarioDAO u = new UsuarioDAO();
            IList<Usuario> listaqtdregistro = u.Lista();

            List<Usuario_login> lusuario_login = new List<Usuario_login>();

            //int[] array = new int[4]; 
            Usuario_login[] ul = new Usuario_login[listaqtdregistro.Count];
            

            TabelasecundariaDAO tdao = new TabelasecundariaDAO();
            PessoaDAO pdao = new PessoaDAO();


            var colegio  = Convert.ToString(Session["Colegio"]);
            var turma    = Convert.ToString(Session["Turma"]);
            var materia  = Convert.ToString(Session["Materia"]);
            var idlogado = Convert.ToString(Session["Loginusuario"]);

            //for (var i = 0; l <= listaqtdregistro.Count; i++ )
            var i = 0;
            foreach (var l in listaqtdregistro)
            {

                IList<Pessoa> lpessoa = pdao.BuscarPessoa(Convert.ToString(l.Id));



                if (lpessoa.Count > 0)
                {
                    if ( 
                          (Convert.ToString(Session["Colegio"]) == Convert.ToString( lpessoa[0].Id_Tabelasecundaria_colegio ) ) && 
                          (Convert.ToString(Session["Turma"])    == Convert.ToString(lpessoa[0].Id_Tabelasecundaria_turma )  )&& 
                          (Convert.ToString(Session["Materia"])  == Convert.ToString(lpessoa[0].Id_Tabelasecundaria_materia ) ) &&
                          (Convert.ToString(Session["Loginusuario"]) != Convert.ToString(lpessoa[0].Id_usuario)) &&
                          (Convert.ToString(Session["Loginusuario"]) == Convert.ToString(lpessoa[0].Id_Tabelasecundaria_usuarioprofessor))
                        )   
                         {

                             ul[i] = new Usuario_login();

                             ul[i].Login = l.Login;
                             ul[i].Id = l.Id;
                             ul[i].Nome = l.Nome;
                             ul[i].Senha = l.Senha;
                             ul[i].Tipousuario = Convert.ToString(l.Tipousuario);
                             ul[i].Id_usuario = Convert.ToInt32(Session["Loginusuario"]);
                             ul[i].Bloqueado = l.Bloqueado;

                        
                             IList<Tabelasecundaria> lcolegio = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(lpessoa[0].Id_Tabelasecundaria_colegio));
                             IList<Tabelasecundaria> lturma = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(lpessoa[0].Id_Tabelasecundaria_turma));

                             ul[i].Id_Tabelasecundaria_colegio = lcolegio[0].Id;
                             ul[i].Id_Tabelasecundaria_turma = lturma[0].Id;
                             ul[i].nomecolegio = lcolegio[0].Descricao;
                             if (ul[i].nomecolegio == "")
                               { ul[i].nomecolegio = "Nenhum"; }
                             ul[i].nometurma = lturma[0].Descricao;
                             if (ul[i].nometurma == "")
                               { ul[i].nometurma = "Nenhum"; }
                             lusuario_login.Add(ul[i]);
                             i = i + 1; 
                    }
                
                }
                //else
                //{
                    //ul[i].Id_Tabelasecundaria_colegio = 0;
                    //ul[i].Id_Tabelasecundaria_turma = 0;
                    //ul[i].nomecolegio = "Nenhum";
                    //ul[i].nometurma = "Nenhum";

                //}
            }

            return View(lusuario_login.ToPagedList(paginanumero, paginatamanho));

            
             
            //return View(listaqtdregistro.ToPagedList(paginanumero, paginatamanho));
           
        }


        public ActionResult Excluir(int id)
        {
            UsuarioDAO u = new UsuarioDAO();
            var u1 = u.BuscarPorId(id);
            u.Excluir(u1);
            return RedirectToAction("Index");
        }


        public ActionResult Adicionar(string acao = "perfil")
        {
            //return View();
            Usuario_login u = new Usuario_login();
            string  email  = "";
            if (acao != "perfil")
            { 
              var arrayacao = acao.Split(';');
              acao  = arrayacao[0];
              email = arrayacao[1];
            }
            //email = "";
            u.Login = email;

            //ViewBag.Habilitarcaixalogin = "S";
            //if (email.Length > 0)
            //{
            //    ViewBag.Habilitarcaixalogin = "N"; 
            //}
            ViewBag.Operacao = "Adicionar";
            ViewBag.statussitema = Session["statussistema"];

            //Session["statussistema"] = "logado";
            if ( (acao == "cadastroprofessor") )
            { 
                ViewBag.Titulotela = "Cadastro Professor";
                u.Tipousuario = "Professor";
            }

            if ((acao == "cadastroaluno"))
            {
                ViewBag.Titulotela = "Cadastro Aluno";
                u.Tipousuario = "Aluno";
            }
            if ( (acao == "perfil") )
              ViewBag.Titulotela = "Perfil Usuário";

            ViewBag.acao = acao;
            //ViewBag.Mensagemerro = "";
            ViewBag.Mensagemsucesso = "";

            TabelasecundariaDAO tabdao = new TabelasecundariaDAO();
            ViewBag.listaturma = tabdao.ListaTabelaSecundaria("T");
            ViewBag.listacolegio = tabdao.ListaTabelaSecundaria("C");
            ViewBag.listamateria = tabdao.ListaTabelaSecundaria("M");
            
            UsuarioDAO udao = new UsuarioDAO();
            var dusuario = udao.ListaUsuario_TipoUsuario("P");

            PessoaDAO pdao1 = new PessoaDAO();
            var nomecolegio = "";
            var nomemateria = "";
            var nometurma = "";

            for (var i = 0; i <= dusuario.Count - 1; i++)
            {
                var dadospessoaprofessor = pdao1.BuscarPessoaprofessor(Convert.ToString(dusuario[i].Id));
                if (dadospessoaprofessor.Count > 0)
                {
                    TabelasecundariaDAO tdao1 = new TabelasecundariaDAO();
                    var lcolegio = tdao1.ListaTabelaSecundariaBuscaporId(Convert.ToString(dadospessoaprofessor[0].Id_Tabelasecundaria_colegio));
                    if (lcolegio.Count > 0)
                    {
                        nomecolegio = lcolegio[0].Descricao;
                    }
                    var lmateria = tdao1.ListaTabelaSecundariaBuscaporId(Convert.ToString(dadospessoaprofessor[0].Id_Tabelasecundaria_materia));
                    nomemateria = "";
                    if (lmateria.Count > 0)
                    {
                        nomemateria = lmateria[0].Descricao;
                    }
                    var lturma = tdao1.ListaTabelaSecundariaBuscaporId(Convert.ToString(dadospessoaprofessor[0].Id_Tabelasecundaria_turma));
                    nometurma = "";
                    if (lturma.Count > 0)
                    {
                        nometurma = lturma[0].Descricao;
                    }

                }
                dusuario[i].Nome = dusuario[i].Nome + " - " + nomecolegio + " - " + nomemateria + " - " + nometurma;
            }
            ViewBag.listaprofessor = dusuario;

            ViewBag.Mensagemerro = "";
            ViewBag.Mensagemsucesso = "";
            
            return View(u);
        }

        [HttpPost]
        public ActionResult Adicionar(Usuario_login u)
        {
            if (1==1)
            {
                //depois de salvar volta para atela de login
     
                var idfacebook = "-1";
                if (Session["Fbdadosusuario"] != null)
                {
                    FacebookController _fu = new FacebookController();
                    var dados = _fu.Criafacebook_usuario(Convert.ToString(Session["Fbdadosusuario"]),
                                         Convert.ToString(Session["Fbdadosfotoperfil"]));

                    //if (dados.email != "")
                    //{
                    //    u.Login = dados.email;
                    //}
                    
                    idfacebook = dados.id;
                }

                Usuario u1 = new Usuario();

                u1.Id = 0;
                
                u1.Login = u.Login;
                
                u1.Nome = u.Nome;
                u1.Senha = u.Senha;
                u1.facebook_usuario_id = idfacebook;
                u.Bloqueado = 'N';
                if (u.Tipousuario == "Professor")
                {
                    u1.Tipousuario = 'P';
                    u.Bloqueado = 'N';
                }
                if (u.Tipousuario == "Aluno")
                {
                    u1.Tipousuario = 'A';
                    u.Bloqueado = 'S';
                }


                if (Session["statussistema"] == "naologado")
                {
                }
                else
                {
                }
                ViewBag.tipousuario = Session["Tipousuario"];
                u1.Bloqueado = u.Bloqueado;

                UsuarioDAO udao = new UsuarioDAO();

                if (ModelState.IsValid)
                {
                    var idusuario = udao.Salvar(u1);
                    u1.Id = idusuario;
                    ViewBag.Mensagemsucesso = "Usuário cadastrado com sucesso";
                    ViewBag.Mensagemerro = "";
                }
                else
                {
                    if (u.Tipousuario == "Professor")
                        ViewBag.Titulotela = "Cadastro Professor";
                    if (u.Tipousuario == "Aluno")
                        ViewBag.Titulotela = "Cadastro Aluno";  
                    
                    ViewBag.Mensagemsucesso = "";
                    ViewBag.Mensagemerro = "Erro";
                    
                    return View(u);
                }


            //   if (Session["statussistema"] == "logado")
                {
                    PessoaDAO pdao = new PessoaDAO();
                    Pessoa pess = new Pessoa();

                    if (u.Id_usuario > 0)
                    {
                       var pessoaprofessor =  pdao.BuscarPessoaprofessor(Convert.ToString(u.Id_usuario));
                       u.Id_Tabelasecundaria_colegio = pessoaprofessor[0].Id_Tabelasecundaria_colegio;
                       u.Id_Tabelasecundaria_materia = pessoaprofessor[0].Id_Tabelasecundaria_materia;
                       u.Id_Tabelasecundaria_turma   = pessoaprofessor[0].Id_Tabelasecundaria_turma;

                    }


                    pess.Id_usuario = u1.Id;
                    pess.Id_Tabelasecundaria_colegio = u.Id_Tabelasecundaria_colegio;//Convert.ToInt32(Session["Colegio"]);
                    pess.Id_Tabelasecundaria_turma = u.Id_Tabelasecundaria_turma;//Convert.ToInt32(Session["Turma"]);
                    pess.Id_Tabelasecundaria_usuarioprofessor = u.Id_usuario;//Convert.ToInt32(Session["Loginusuario"]);
                    pess.Id_Tabelasecundaria_materia = u.Id_Tabelasecundaria_materia;//Convert.ToInt32(Session["Materia"]); //u.Id_Tabelasecundaria_materia; 
                    pdao.Salvar(pess, ViewBag.Operacao);

                }

                
                ViewBag.Mensagemerro = "";
                
                //pdao.Salvar(p1, ViewBag.Operacao);
                if (ModelState.IsValid)
                {
                    if (Session["statussistema"] == "naologado")
                    { return RedirectToAction("Loginusuario"); }
                    else
                    { return RedirectToAction("Index"); }
                }
                else
                {
                    
                    return View(u); 
                }
            }
        }

        
        
        public ActionResult Form()
        {

            return View();
        }

        public ActionResult principal()
        {

            ViewBag.tipousuario = Session["Tipousuario"];
            
            TotalDao Tdao = new TotalDao();
            ViewBag.totalexercicioscorretos = Tdao.TotalexercicioscertosProfessor( Convert.ToString(  Session["Loginusuario"] ) );
            ViewBag.totalexercicioserrados = Tdao.TotalexercicioserradosProfessor( Convert.ToString(  Session["Loginusuario"]  ) );            
            
            ViewBag.totalexercicios = Tdao.TotalexerciciosProfessor( Convert.ToString(  Session["Loginusuario"]  ) );
            ViewBag.totalexerciciosrespondidos = Tdao.TotalexerciciosrespondidosProfessor(Convert.ToString(Session["Loginusuario"]));

            ViewBag.totalexerciciossemresposta = Tdao.TotalexerciciosNovosProfessor(Convert.ToString(Session["Loginusuario"]));

            ViewBag.Percentualrealizado = 0;
            if (ViewBag.totalexercicios > 0)
                ViewBag.Percentualrealizado = Convert.ToInt32 ( (ViewBag.totalexerciciosrespondidos / ViewBag.totalexercicios) * 100 );

            ViewBag.Percentualcerto = 0;
            if (ViewBag.totalexercicios > 0)
                ViewBag.Percentualcerto =    Convert.ToInt32( (ViewBag.totalexercicioscorretos / ViewBag.totalexercicios) * 100 );

            ViewBag.Percentualincorreto = 0;
            if (ViewBag.totalexercicios > 0)
                ViewBag.Percentualincorreto = Convert.ToInt32( (ViewBag.totalexercicioserrados / ViewBag.totalexercicios) * 100 );

            return View();
        }


        public ActionResult Alterar(int id)
        {
            ViewBag.idalteracao = id;

            ViewBag.statussitema = Session["statussistema"];
            ViewBag.Mensagemsucesso = "";
            ViewBag.Mensagemerro = "";
            ViewBag.statussistema = Session["statussistema"];
            

            Usuario_login u = new Usuario_login();
            UsuarioDAO pdao = new UsuarioDAO();
            var p2 = pdao.BuscarPorId(id);
            u.Bloqueado = p2.Bloqueado;
            u.Id = id;
            u.Login = p2.Login;
            u.Nome = p2.Nome;
            u.Senha = p2.Senha;
            u.Tipousuario = Convert.ToString(p2.Tipousuario);
            

            ViewBag.Habilitarcaixalogin = "N";//sempre vir desabilitado o botao de alterar


            if (u.Tipousuario == "P")
                u.Tipousuario = "Professor";
            else
                u.Tipousuario = "Aluno";

            if (u.Tipousuario == "Professor")
                ViewBag.acao = "cadastroprofessor";
            else
                ViewBag.acao = "cadastroaluno";

            if (u.Tipousuario == "Professor")
                ViewBag.Titulotela = "Cadastro Professor";
            else
                ViewBag.Titulotela = "Cadastro Aluno";
            
            PessoaDAO pssdao = new PessoaDAO();
            var pessid = pssdao.BuscarPessoa(Convert.ToString(id));
            if (pessid.Count == 0)
                pessid = pssdao.BuscarPessoaprofessor(Convert.ToString(id));

            Pessoa pess = new Pessoa();
            if (pessid.Count == 0)
                pess.Id = 0;
            else
                pess.Id = pessid[0].Id;

            if (pessid.Count > 0)
            { 
            u.Id_Tabelasecundaria_materia = pessid[0].Id_Tabelasecundaria_materia;
            u.Id_Tabelasecundaria_colegio = pessid[0].Id_Tabelasecundaria_colegio;
            u.Id_Tabelasecundaria_turma   = pessid[0].Id_Tabelasecundaria_turma;
            }
            else
            {
                u.Id_Tabelasecundaria_materia = 0;
                u.Id_Tabelasecundaria_colegio = 0;
                u.Id_Tabelasecundaria_turma   = 0;
            
            
            }
            //if (Session["statussistema"] != "naologado")
            //{
                TabelasecundariaDAO tabdao = new TabelasecundariaDAO();
                ViewBag.listaturma = tabdao.ListaTabelaSecundaria("T");
                ViewBag.listacolegio = tabdao.ListaTabelaSecundaria("C");
                ViewBag.listamateria = tabdao.ListaTabelaSecundaria("M");
            //}
            

            
            return View(u);
        }
        
        [HttpPost]
        public ActionResult Alterar(int id, Usuario_login u)
        {
           ViewBag.Operacao = "Alterar";
           ViewBag.Mensagemsucesso = "";
           ViewBag.Mensagemerro = "";
           //return View();
           
           //UsuarioDAO udao = new UsuarioDAO();
           //var usuario = udao.BuscarPorId(id);
           //u.Login     = usuario.Login;
           //u.Bloqueado = usuario.Bloqueado;

           ViewBag.statussitema = Session["statussistema"];

           //if (!(ModelState.IsValid))
           //{
           //    ViewBag.Mensagemsucesso = "";
           //    ViewBag.Mensagemerro = "Sim";
           //    return View(u);
           //}
           //else
           {
              
               //depois de salvar volta para atela de login
               Usuario u1 = new Usuario();
               UsuarioDAO pdao1 = new UsuarioDAO();
 
               var p21 = pdao1.BuscarPorId(u.Id);
 
               u1.Id = u.Id;
               u1.Login = p21.Login;
               u1.Nome = u.Nome;
               u1.Senha = u.Senha;
               u1.facebook_usuario_id = p21.facebook_usuario_id;

               ViewBag.Possuisessao = Session.Count;
               if (Session["statussistema"] == "naologado")
               {
                   ViewBag.Possuisessao = 0;
               }

               if (Session["statussistema"] == "naologado")
               {
                   if (u.Tipousuario == "Professor")
                   {
                       u1.Tipousuario = 'P';
                       u.Bloqueado = 'S';
                   }
                   if (u.Tipousuario == "Autor")
                       u1.Tipousuario = 'A';
                   if (u.Tipousuario == "Leitor")
                       u1.Tipousuario = 'L';
               }
               else
               {
                   u1.Tipousuario = p21.Tipousuario;
                   
               }
               ViewBag.tipousuario = Session["Tipousuario"];
               u1.Bloqueado = p21.Bloqueado;

               UsuarioDAO udao = new UsuarioDAO();


               udao.Salvar(u1);

               if (Session["statussistema"] == "logado")
               {
                   PessoaDAO pdao = new PessoaDAO();
                   var pessid = pdao.BuscarPessoa(Convert.ToString(id));
                   if (pessid.Count == 0)
                      pessid = pdao.BuscarPessoaprofessor(Convert.ToString(id));
                   
                   Pessoa pess = new Pessoa();
                   if (pessid.Count == 0)
                       pess.Id = 0;
                   else
                       pess.Id = pessid[0].Id;

                   if (Convert.ToInt32( Session["Materia"] ) == 0)
                     Session["Materia"] = u.Id_Tabelasecundaria_materia;
                   if (Convert.ToInt32(Session["Colegio"]) == 0)
                     Session["Colegio"] = u.Id_Tabelasecundaria_colegio;
                   if (Convert.ToInt32( Session["Turma"] ) == 0)
                     Session["Turma"]   = u.Id_Tabelasecundaria_turma;

                   if (u.Id_Tabelasecundaria_materia == 0)
                       u.Id_Tabelasecundaria_materia = Convert.ToInt32( Session["Materia"] );
                   if (u.Id_Tabelasecundaria_colegio == 0)
                       u.Id_Tabelasecundaria_colegio = Convert.ToInt32(Session["Colegio"]);
                   if (u.Id_Tabelasecundaria_turma == 0)
                       u.Id_Tabelasecundaria_turma = Convert.ToInt32(Session["Turma"]);

                   
                   pess.Id_usuario = u1.Id;
                   pess.Id_Tabelasecundaria_materia          = u.Id_Tabelasecundaria_materia;
                   pess.Id_Tabelasecundaria_colegio          = u.Id_Tabelasecundaria_colegio;
                   pess.Id_Tabelasecundaria_turma            = u.Id_Tabelasecundaria_turma ;
                   pess.Id_Tabelasecundaria_usuarioprofessor = Convert.ToInt32(Session["Loginusuario"]);

                   //if ( 
                   //     (u.Id_Tabelasecundaria_materia > 0) && 
                   //     (u.Id_Tabelasecundaria_colegio > 0) &&  
                   //     (u.Id_Tabelasecundaria_turma   > 0) 
                   //   ) 
                   //  {
                       pdao.Salvar(pess, ViewBag.Operacao);
                   //  }
               }


               //pdao.Salvar(p1, ViewBag.Operacao);
               if (Session["statussistema"] == "naologado")
                 { return RedirectToAction("Loginusuario"); }
               else
               {
                   
                   u.Id_Tabelasecundaria_materia = Convert.ToInt32 ( Session["Materia"] );
                   u.Id_Tabelasecundaria_turma   = Convert.ToInt32 ( Session["Turma"] );
                   u.Id_Tabelasecundaria_colegio = Convert.ToInt32 ( Session["Colegio"] );
                    
                   TabelasecundariaDAO tdao = new TabelasecundariaDAO();
                   Usuario_login ul = new Usuario_login();
                   ul.Id = u1.Id;
                   ul.Id_Tabelasecundaria_colegio = u.Id_Tabelasecundaria_colegio;
                   ul.Id_Tabelasecundaria_materia = u.Id_Tabelasecundaria_materia;
                   ul.Id_Tabelasecundaria_turma   = u.Id_Tabelasecundaria_turma;
                   ul.Id_usuario = u.Id_usuario;
                   ul.Login = p21.Login;
                   ul.Senha = u.Senha;
                   ul.Nome = u.Nome;


                   
                   ul.Tipousuario = u.Tipousuario;
                   if (u.Id_Tabelasecundaria_colegio > 0)
                   { 
                     IList<Tabelasecundaria> ltab = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(u.Id_Tabelasecundaria_colegio));
                     ul.nomecolegio = ltab[0].Descricao;
                   }
                   if (u.Id_Tabelasecundaria_turma > 0)
                   { 
                     IList<Tabelasecundaria> ltab1 = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(u.Id_Tabelasecundaria_turma));
                     ul.nometurma   = ltab1[0].Descricao;
                   }
                   if (u.Id_Tabelasecundaria_materia > 0)
                   { 
                     IList<Tabelasecundaria> ltab2 = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(u.Id_Tabelasecundaria_materia));
                     ul.nomemateria   = ltab2[0].Descricao;
                   }
                   
                   TabelasecundariaDAO tabdao = new TabelasecundariaDAO();
                   ViewBag.listaturma = tabdao.ListaTabelaSecundaria("T");
                   ViewBag.listacolegio = tabdao.ListaTabelaSecundaria("C");
                   ViewBag.listamateria = tabdao.ListaTabelaSecundaria("M");
                   
                   ViewBag.Mensagemsucesso = "Alteração efetuada com sucesso !!!";
                   return View(ul); 
               
               }
           }
        
        }

        public void SalvarDados(Usuario p)
        {
            UsuarioDAO dao = new UsuarioDAO();
            dao.Salvar(p);
        }

        public Usuario Buscausuariosistema(string login , string campopesquisado = "Login")
        {
           
            GenericDAO<Usuario> ul = new GenericDAO<Usuario>();
            var usu = ul.ConsultagenericaPesquisaUmachave("Usuario", " pu ", "pu." + campopesquisado, login);
            if (usu.Count == 0)
            {
                return null;
            }
            else
            {
                  return usu[0];
            }
        }

        public ActionResult LoginUsuario(string login, string senha)
        {
            ViewBag.Operacao = "Adicionar";
            ViewBag.MensagemBloqueado = "";
            Session["statussistema"] = "naologado";
            var idfacebook = "";
            if ( (Session["Fbdadosusuario"] != null) && (login == null) && (senha == null)  )
              { 
                FacebookController _fu = new FacebookController();
                var dados = _fu.Criafacebook_usuario(Convert.ToString(Session["Fbdadosusuario"]), 
                                                 Convert.ToString(Session["Fbdadosfotoperfil"]));
                login = dados.email;
                idfacebook =  dados.id; //esse eh o id do facebook
                var usuario = Buscausuariosistema(idfacebook, "facebook_usuario_id");
                if (usuario == null)
                {
                    usuario = Buscausuariosistema(login);          
                }

                if (usuario == null)
                {
                    senha = "*.*.*.*";
                }
                else
                {
                    login = usuario.Login;
                    senha = usuario.Senha;
                }

                if (senha == "*.*.*.*")
                  {

                    return RedirectToAction("LinkCadastroNaoPossuifacebook", "Usuario"); 

                    login = null;
                    senha = null;
                  }
              }

            if (  (login != null) && (senha != null)  )
            {
                UsuarioDAO dao = new UsuarioDAO();
                IList<Usuario> lusuario = dao.Loginusuario(login, senha);
               if (lusuario.Count > 0)
                {
                    FacebookController _fu = new FacebookController();
                    //_fu.publicar();
                    if (Session["FbuserToken"] != null)
                      { 
                         _fu.PublicarMensagem(Session["FbuserToken"].ToString() ,
                             "O usuário " + lusuario[0].Nome + " acabou de entrar no sistema Webescola" ,
                             "http://webescola.smartechsolution.com.br/");
                      }
                    Session["statussistema"] = "logado";
                    Session["Loginusuario"] = lusuario[0].Id;
                    Session["Loginlogin"]   = lusuario[0].Login;
                    Session["Loginnome"]    = lusuario[0].Nome;
                    if (lusuario[0].Tipousuario == 'A' )
                      Session["Tipousuario"]  = "Autor";

                    if (lusuario[0].Tipousuario == 'L')
                        Session["Tipousuario"] = "Leitor";
                    
                   if (lusuario[0].Tipousuario == 'P')
                        Session["Tipousuario"] = "Professor";
                    
                   ViewBag.Tipousuario = Session["Tipousuario"];

                   PessoaDAO pessdao = new PessoaDAO();
                   if (lusuario[0].Tipousuario == 'P')
                    {
                        var pessoa = pessdao.BuscarPessoaprofessor(Convert.ToString(lusuario[0].Id));

                        Session["Materia"] = 0;
                        Session["Turma"]   = 0;
                        Session["Colegio"] = 0;

                        if (pessoa.Count > 0)
                        {
                            Session["Materia"] = pessoa[0].Id_Tabelasecundaria_materia;
                            Session["Turma"]   = pessoa[0].Id_Tabelasecundaria_turma;
                            Session["Colegio"] = pessoa[0].Id_Tabelasecundaria_colegio;
                        }

                    }

                   if (lusuario[0].Bloqueado == 'S')
                    {

                        if (lusuario[0].Tipousuario == 'P')
                        {
                            ViewBag.MensagemBloqueado = Session["Loginnome"] + " está bloqueado.";
                            login = null;
                            senha = null;
                        }
                        if (lusuario[0].Tipousuario == 'A')
                        {
                            ViewBag.MensagemBloqueado = Session["Loginnome"] + " está bloqueado. Aguarde o desbloqueio Professor";
                            login = null;
                            senha = null;
                        }
                       
                       return View();
                    }
                    else
                    { 
                        return RedirectToAction("principal", "Usuario"); 
                    }

                   
               }
                else
                  {
                      ViewBag.MensagemBloqueado = "Usuário ou senha inválido!!! Tente novamente.";      
                      return View();
                  }    
                   
            }
            return View();     
        
        }


        public ActionResult LinkCadastroNaoPossuifacebook()
        {
            ViewBag.Mensagemnaopossuiusuario = "Você Não possui Usuário cadastrado";

            Usuario_login u = new Usuario_login();
            Session["statussistema"] = "naologado";
            ViewBag.statussistema    = "naologado";

            if (Session["Fbdadosusuario"] != null)
            {
                FacebookController _fu = new FacebookController();
                var dados = _fu.Criafacebook_usuario(Convert.ToString(Session["Fbdadosusuario"]),
                                     Convert.ToString(Session["Fbdadosfotoperfil"]));


                ViewBag.PrimeiroNome = dados.first_name;
                ViewBag.Sobrenome = dados.last_name;
                ViewBag.Datanascimento = dados.birthday;
                ViewBag.email = dados.email;
                //dados.email = "";
                u.Login = dados.email;
                if (dados.email != "")
                { 
                  ViewBag.Mensagemnaopossuiusuario = "O login " + dados.email +" não possui cadastro."+
                                                     "Confirme seus dados, antes de continuar";
                } 

                if (dados.email == "")
                { 
                  ViewBag.Mensagemnaopossuiusuario = "Não foi possível fazer o login pelo facebook e-mail bloqueado";
                } 


            }

            ViewBag.LoginUsuario      = u.Login;
            ViewBag.cadastroprofessor = "cadastroprofessor";
            ViewBag.cadastroaluno     = "cadastroaluno";

           

            return View( u );        

        }

        public ActionResult AtivarBloquear(int idusuario)
        { 
        
            UsuarioDAO udao = new UsuarioDAO();
            var usuario = udao.BuscarPorId( idusuario );
            if (usuario.Bloqueado == 'S')
              usuario.Bloqueado = 'N';
            else
              usuario.Bloqueado = 'S';

            this.SalvarDados(usuario);

            return RedirectToAction("Index", "Usuario");
        }

        //Api
        public JsonResult JsonLoginUsuario(string login , string senha )
        {
           var l = new List<object>();
           var lp = new List<object>();

            
            if ((login != null) && (senha != null))
            {
                UsuarioDAO dao = new UsuarioDAO();
                IList<Usuario> lusuario = dao.Loginusuario(login, senha);
                if (lusuario.Count > 0)
                {
                    TabelasecundariaDAO tdao = new TabelasecundariaDAO();

                    UsuarioDAO udao = new UsuarioDAO();

                    PessoaDAO pdao = new PessoaDAO();
                    var pessgeral = pdao.BuscarPessoa(Convert.ToString(lusuario[0].Id));

                    var xidusuario = lusuario[0].Id;

                    var xturma = tdao.ListaTabelaSecundariaBuscaporId("0");
                    var xmateria = tdao.ListaTabelaSecundariaBuscaporId("0");
                    var xcolegio = tdao.ListaTabelaSecundariaBuscaporId("0");
                    var xprof = udao.BuscarPorId(0);

                    var xidturma = 0;
                    var xidmateria = 0;
                    var xidcolegio = 0;
                    var xidprof = 0;

                    Pergunta_UsuarioDAO tdao1 = new Pergunta_UsuarioDAO();
                    var qtd = 0; 

                    foreach (var pess in pessgeral)
                    {

                        if (Convert.ToString(pess.Id_Tabelasecundaria_turma) == "")
                            pess.Id_Tabelasecundaria_turma = 0;

                        if (Convert.ToString(pess.Id_Tabelasecundaria_materia) == "")
                            pess.Id_Tabelasecundaria_materia = 0;

                        if (Convert.ToString(pess.Id_Tabelasecundaria_colegio) == "")
                            pess.Id_Tabelasecundaria_colegio = 0;

                        xturma = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(pess.Id_Tabelasecundaria_turma));
                        xmateria = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(pess.Id_Tabelasecundaria_materia));
                        xcolegio = tdao.ListaTabelaSecundariaBuscaporId(Convert.ToString(pess.Id_Tabelasecundaria_colegio));
                        xprof = udao.BuscarPorId(pess.Id_Tabelasecundaria_usuarioprofessor);

                        xidturma   = pess.Id_Tabelasecundaria_turma;
                        xidmateria = pess.Id_Tabelasecundaria_materia;
                        xidcolegio = pess.Id_Tabelasecundaria_colegio;
                        xidprof    = pess.Id_Tabelasecundaria_usuarioprofessor;

                        var lnao = tdao1.ListaPerguntaUsuarionaorespondida(Convert.ToString(xidusuario), 
                                                                           Convert.ToString(xidmateria),
                                                                           Convert.ToString(xidprof));

                        lp.Add(new { turma1= xturma[0].Descricao , 
                                     colegio1= xcolegio[0].Descricao , 
                                     materia1 = xmateria[0].Descricao , 
                                     prof1 = xprof.Nome ,  
                                     idturma1 = xidturma   ,
                                     idmateria1 = xidmateria ,
                                     idcolegio1 = xidcolegio ,
                                     idprofessor1 = xidprof ,
                                     qtdperguntas = lnao.Count

                        }); 

                    }

                    if (lusuario[0].Bloqueado == 'S')
                        lusuario[0].Bloqueado = 'B'; //bloqueado
                    if (lusuario[0].Bloqueado != 'S')
                        lusuario[0].Bloqueado = 'L'; //Liberado

                    var lnao1 = tdao1.ListaPerguntaUsuarionaorespondida(Convert.ToString(xidusuario),
                                                   Convert.ToString(xidmateria),
                                                   Convert.ToString(xidprof));

                    
                    l.Add(new
                    {
                        id = lusuario[0].Id,
                        Situacao = lusuario[0].Bloqueado,
                        Nome = lusuario[0].Nome,
                        turma = xturma[0].Descricao,
                        colegio = xcolegio[0].Descricao,
                        materia = xmateria[0].Descricao,
                        professor = xprof.Nome,
                        qtdperguntas = lnao1.Count,
                        idturma = xidturma   ,
                        idmateria = xidmateria ,
                        idcolegio = xidcolegio ,
                        idprofessor = xidprof    ,
                        
                        listaturma = lp

                    });

                }
                else
                {
                    l.Add(new
                    {
                        id = "-1",
                        Situacao = 0,
                        Nome = "",
                        turma = "",
                        colegio = "",
                        materia = "",
                        professor = ""
                    });
                
                }

            }
            else
            {

                l.Add(new
                {
                    id = "-1",
                    Situacao = 0,
                    Nome = "",
                    turma = "",
                    colegio = "",
                    materia = "",
                    professor = ""
                });

            }
            return Json( l[0] , JsonRequestBehavior.AllowGet);
        
        }

        public ActionResult LoginUsuarioCupom()
        { 
           
           
            if (Session["FbuserToken"] == null)
           {
               FacebookController fbc = new FacebookController();
               Session.Add("controle", "Usuario");
               Session.Add("pagina", "LoginUsuarioCupom");
               
               
                ViewBag.linkloginfacebook = fbc.GetFacebookLoginUrl();

               return View();
           }
           else
           {
               FacebookController _fu = new FacebookController();
               var dados = _fu.Criafacebook_usuario(Convert.ToString(Session["Fbdadosusuario"]),
                                                Convert.ToString(Session["Fbdadosfotoperfil"]));
  
                
                //abri a pagina de opcoes do sistema
               return RedirectToAction("Menucupom", "Cupom");
           }
           
        }
        
      
       /* public ActionResult LoginUsuarioCupom()
        {
            FacebookController fbc = new FacebookController();
            fbc.Criafacebook_usuario(
                   Convert.ToString(Session["Fbdadosusuario"]), Convert.ToString(Session["Fbdadosfotoperfil"]));

            fbc.GravarUsuarioFacebook("Login Sistema");

            //LoginUsuario(string login, string senha)
            return RedirectToAction("Loginusuario", "Usuario");
        }*/
        
        //localhost:60661/usuario/BuscaUsuarioportipo?tipo=P
        public JsonResult BuscaUsuarioportipo(string tipo)
        {
            UsuarioDAO udao = new UsuarioDAO();
            var ulista = udao.ListaUsuario_TipoUsuario(tipo);
            var l = new List<object>();


            if (ulista.Count > 0)
            {

                return Json(ulista, JsonRequestBehavior.AllowGet);

            }
            else
            {
                
                l.Add(new
                {
                    id = "-1",
                    Situacao = 0,
                    Nome = "",
                    turma = "",
                    colegio = "",
                    materia = "",
                    professor = ""
                });

                
                return Json( l[0] , JsonRequestBehavior.AllowGet);

            }
        
        }

        //localhost:60661/usuario/adicionar?xlogin="santana@msn.com.br"&xsenha="12345"&xnome="juninho"&xidprofessor="24"&xtipousuario="Aluno"
        //localhost:60661/usuario/Apiadicionar?xlogin=santana@msn.com.br&xsenha=12345&xnome=juninho&xidprofessor=24&xtipousuario=Aluno
        public Int32 ApiAdicionar(string xlogin , string xsenha , string xnome , string xidprofessor, string xtipousuario)
        {

               UsuarioDAO _udao = new UsuarioDAO();
               //IList<Usuario> lusuario1 = _udao.Loginusuario(xlogin, xsenha);
               //if (lusuario1.Count > 0) {
               //    return -2;
               //}

               IList<Usuario> lusuario1 = _udao.ExisteLoginUsuario(xlogin);
               if (lusuario1.Count > 0)
               {
                   return -2;
               }
               

                Usuario u1 = new Usuario();

                u1.Id = 0;

                u1.Login = xlogin;

                u1.Nome = xnome;
                u1.Senha = xsenha;
                u1.facebook_usuario_id = "-1";
                u1.Bloqueado = 'N';
                if (xtipousuario == "Professor")
                {
                    u1.Tipousuario = 'P';
                    u1.Bloqueado = 'N';
                }
                if (xtipousuario == "Aluno")
                {
                    u1.Tipousuario = 'A';
                    u1.Bloqueado = 'S';
                }

                UsuarioDAO udao = new UsuarioDAO();

                if (ModelState.IsValid)
                {
                    var idusuario = udao.Salvar(u1);
                    u1.Id = idusuario;
                }
                else
                {
                    return -1;
                }


                //   if (Session["statussistema"] == "logado")
                {
                    PessoaDAO pdao = new PessoaDAO();
                    Pessoa pess = new Pessoa();

                    var idcolegio = 0;
                    var idmateria = 0;
                    var idturma = 0;

                    if (Convert.ToInt32( xidprofessor ) > 0)
                    {
                        var pessoaprofessor = pdao.BuscarPessoaprofessor(xidprofessor);
                        idcolegio = pessoaprofessor[0].Id_Tabelasecundaria_colegio;
                        idmateria = pessoaprofessor[0].Id_Tabelasecundaria_materia;
                        idturma = pessoaprofessor[0].Id_Tabelasecundaria_turma;
                    }


                    pess.Id_usuario = u1.Id;
                    pess.Id_Tabelasecundaria_colegio = idcolegio; //u1.Id_Tabelasecundaria_colegio;//Convert.ToInt32(Session["Colegio"]);
                    pess.Id_Tabelasecundaria_turma =   idturma; //u.Id_Tabelasecundaria_turma;//Convert.ToInt32(Session["Turma"]);
                    pess.Id_Tabelasecundaria_usuarioprofessor =  Convert.ToInt32(xidprofessor);//u.Id_usuario;//Convert.ToInt32(Session["Loginusuario"]);
                    pess.Id_Tabelasecundaria_materia = idmateria; //u.Id_Tabelasecundaria_materia;//Convert.ToInt32(Session["Materia"]); //u.Id_Tabelasecundaria_materia; 
                    pdao.Salvar(pess, "Incluir");

                }


                //pdao.Salvar(p1, ViewBag.Operacao);
                if (ModelState.IsValid)
                {
                   return u1.Id;
                }
                else
                {

                    return -1;
                }
            }
        

        
        
        public ActionResult LoginUsuario_google(string login, string senha)
        {
            ViewBag.Operacao = "Adicionar";
            ViewBag.MensagemBloqueado = "";
            Session["statussistema"] = "naologado";
            if ((login != null) && (senha != null))
            {
                UsuarioDAO dao = new UsuarioDAO();
                IList<Usuario> lusuario = dao.Loginusuario(login, senha);
                if (lusuario.Count > 0)
                {

                    Session["statussistema"] = "logado";
                    Session["Loginusuario"] = lusuario[0].Id;
                    Session["Loginlogin"] = lusuario[0].Login;
                    Session["Loginnome"] = lusuario[0].Nome;
                    if (lusuario[0].Tipousuario == 'A')
                        Session["Tipousuario"] = "Autor";

                    if (lusuario[0].Tipousuario == 'L')
                        Session["Tipousuario"] = "Leitor";

                    if (lusuario[0].Tipousuario == 'P')
                        Session["Tipousuario"] = "Professor";

                    ViewBag.Tipousuario = Session["Tipousuario"];

                    PessoaDAO pessdao = new PessoaDAO();
                    if (lusuario[0].Tipousuario == 'P')
                    {
                        var pessoa = pessdao.BuscarPessoaprofessor(Convert.ToString(lusuario[0].Id));
                        Session["Materia"] = pessoa[0].Id_Tabelasecundaria_materia;
                        Session["Turma"] = pessoa[0].Id_Tabelasecundaria_turma;
                        Session["Colegio"] = pessoa[0].Id_Tabelasecundaria_colegio;
                    }

                    if (lusuario[0].Bloqueado == 'S')
                    {
                        ViewBag.MensagemBloqueado = Session["Loginnome"] + " está bloqueado.";
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("principal", "Usuario");
                    }


                }
                else
                {
                    ViewBag.MensagemBloqueado = "Usuário ou senha inválido!!! Tente novamente.";
                    return View();
                }

            }
            return View();

        }

    }
}