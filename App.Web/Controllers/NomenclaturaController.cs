using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using App.Dados.Interfaces;
using App.Modelo;
using System.Web.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;


namespace App.Web.Controllers
{
	public class NomenclaturaController : ApiControllerBase
	{
		public NomenclaturaController(IUnidadeDeTrabalho unidadeDeTrabalho)
		{
			UnidadeDeTrabalho = unidadeDeTrabalho;
		}

		// GET /api/Nomenclatura
		public IHttpActionResult Get(int idTipoItem, int idCliente)
		{
			var skip = Request.GetQueryStringValue<int?>("skip", null);
			var take = Request.GetQueryStringValue<int?>("take", null);
			var ativo = Request.GetQueryStringValue<bool?>("ativo", null);
			var busca = Request.GetQueryStringValue<string>("busca", null);

			var query = UnidadeDeTrabalho.Nomenclatura.ObterTodos(idTipoItem, idCliente);

			// sem paginar
			if (!skip.HasValue || !take.HasValue)
				return Ok(query.ToList());

			// paginação
			return Ok(new
			{
				count = query.Count(),
				results = query.Skip(skip.Value).Take(take.Value)
			});
		}

		// GET /api/Nomenclatura/nomenclaturaDoCliente
		[HttpPost, ActionName("nomenclaturaCliente")]
		public dynamic nomenclaturaCliente(NomenclaturaCliente nomenclaturaCliente)
		{
			foreach (var item in nomenclaturaCliente.ItensColeta)
			{
				var itemBD = UnidadeDeTrabalho.ItemColeta.ObterPorID(item.ID);
				switch ((TipoItemColetaEnum)itemBD.IdTipoItemColeta)
				{
					case TipoItemColetaEnum.Display:
						var nomenclaturasDisplay = UnidadeDeTrabalho.Nomenclatura_Display.ObterTodos().Where(x => x.IdCliente == nomenclaturaCliente.idCliente);
						item.Nome = nomenclaturasDisplay.Where(y => y.Id == item.ID).Any() ? nomenclaturasDisplay.Where(y => y.Id == item.ID).FirstOrDefault().Nome : itemBD.Nome;
						break;
					case TipoItemColetaEnum.Merchandising:
						var nomenclaturasMerchandising = UnidadeDeTrabalho.Nomenclatura_Merchandising.ObterTodos().Where(x => x.IdCliente == nomenclaturaCliente.idCliente);
						item.Nome = nomenclaturasMerchandising.Where(y => y.Id == item.ID).Any() ? nomenclaturasMerchandising.Where(y => y.Id == item.ID).FirstOrDefault().Nome : itemBD.Nome;
						break;
					case TipoItemColetaEnum.MetaProduto:
						var nomenclaturasMetaProduto = UnidadeDeTrabalho.Nomenclatura_MetaProduto.ObterTodos().Where(x => x.IdCliente == nomenclaturaCliente.idCliente);
						item.Nome = nomenclaturasMetaProduto.Where(y => y.Id == item.ID).Any() ? nomenclaturasMetaProduto.Where(y => y.Id == item.ID).FirstOrDefault().Nome : itemBD.Nome;
						break;
					case TipoItemColetaEnum.MultiProduto:
						var nomenclaturasMultiProduto = UnidadeDeTrabalho.Nomenclatura_MultiProduto.ObterTodos().Where(x => x.IdCliente == nomenclaturaCliente.idCliente);
						item.Nome = nomenclaturasMultiProduto.Where(y => y.Id == item.ID).Any() ? nomenclaturasMultiProduto.Where(y => y.Id == item.ID).FirstOrDefault().Nome : itemBD.Nome;
						break;
					case TipoItemColetaEnum.Produto:
						var nomenclaturasProduto = UnidadeDeTrabalho.Nomenclatura_Produto.ObterTodos().Where(x => x.IdCliente == nomenclaturaCliente.idCliente);
						item.Nome = nomenclaturasProduto.Where(y => y.Id == item.ID).Any() ? nomenclaturasProduto.Where(y => y.Id == item.ID).FirstOrDefault().Nome : itemBD.Nome;
						break;

					default:
						return null;
				}


				item.NomeOriginal = itemBD != null ? itemBD.Nome : "";
			}

			foreach (var item in nomenclaturaCliente.Locais)
			{
				var itemBD = UnidadeDeTrabalho.LocalExibicao.ObterPorID(item.ID);
				var nomenclaturasLocalExibicao = UnidadeDeTrabalho.Nomenclatura_LocalExibicao.ObterTodos().Where(x => x.IdCliente == nomenclaturaCliente.idCliente);
				item.Nome = nomenclaturasLocalExibicao.Where(y => y.Id == item.ID).Any() ? nomenclaturasLocalExibicao.Where(y => y.Id == item.ID).FirstOrDefault().Nome : itemBD.Nome;
				item.NomeOriginal = itemBD != null ? itemBD.Nome : "";
			}

			return nomenclaturaCliente;
		}

		// GET /api/Nomenclatura/nomenclaturaImportacaoCSV
		[HttpPost, ActionName("nomenclaturaImportacaoProduto")]
		[Authorize(Roles = "Administrador,Cadastro,Técnico")]
		public dynamic nomenclaturaImportacaoProduto(NomenclaturaImportacaoProduto nomenclaturaImportacaoProduto)
		{
			foreach (var item in nomenclaturaImportacaoProduto.Fabricantes)
			{
				if (String.IsNullOrEmpty(item.Nome))
					continue;

				var fabricante = UnidadeDeTrabalho.Fabricante.ObterTodos().Where(x => x.Nome == item.Nome).FirstOrDefault();
				if (fabricante != null)
					item.Id = fabricante.Id;
			}

			foreach (var item in nomenclaturaImportacaoProduto.Marcas)
			{
				if (String.IsNullOrEmpty(item.Nome))
					continue;

				var marca = UnidadeDeTrabalho.Marca.ObterTodos().Where(x => x.Nome == item.Nome).FirstOrDefault();
				if (marca != null)
					item.Id = marca.Id;
			}

			foreach (var item in nomenclaturaImportacaoProduto.Submarcas)
			{
				if (String.IsNullOrEmpty(item.Nome))
					continue;

				var submarca = UnidadeDeTrabalho.Submarca.ObterTodos().Where(x => x.Nome == item.Nome).FirstOrDefault();
				if (submarca != null)
					item.Id = submarca.Id;
			}

			foreach (var item in nomenclaturaImportacaoProduto.Codigos)
			{
				if (String.IsNullOrEmpty(item.Nome))
					continue;

				int codigo = 0;
				int.TryParse(item.Nome, out codigo);
				var produto = UnidadeDeTrabalho.Produto.ObterTodos().Where(x => x.CodigoInterno == codigo).FirstOrDefault();
				if (produto != null)
					item.Id = produto.Id;
			}

			return nomenclaturaImportacaoProduto;
		}

		//GET /api/Nomenclatura/#
		public IHttpActionResult Get(byte idTipoItem, int idCliente, int idItem)
		{
			var model = UnidadeDeTrabalho.Nomenclatura.ObterPorID(idTipoItem, idCliente, idItem);
			if (model != null) return Ok(model);

			return NotFound();
		}

		// PUT /api/Nomenclatura/#
		[Authorize(Roles = "Administrador,Cadastro,Técnico")]
		public void Put(int idCliente, byte idTipoItem, int idItem, dynamic nomenclatura)
		{
			nomenclatura.idCliente = idCliente;
			nomenclatura.id = idItem;
			UnidadeDeTrabalho.Nomenclatura.AdicionarOuAtualizar(idTipoItem, nomenclatura);
			UnidadeDeTrabalho.Commit();
		}

		// DELETE /api/Nomenclatura/#
		[Authorize(Roles = "Administrador,Cadastro,Técnico")]
		public void Delete(int idCliente, byte idTipoItem, int idItem)
		{
			UnidadeDeTrabalho.Nomenclatura.Remover(idTipoItem, idCliente, idItem);
			UnidadeDeTrabalho.Commit();
		}


		// GET /api/Nomenclatura/DownloadNomenclaturaProdutos
		[HttpGet, ActionName("DownloadNomenclaturaProdutos")]
		public System.Net.Http.HttpResponseMessage DownloadNomenclaturaProdutos(int idCliente)
		{
			var nomenclatura = UnidadeDeTrabalho.Nomenclatura.ObterTodos((int)TipoCadastroEnum.Produto, idCliente);
			List<dynamic> lista = nomenclatura.ToList();
			GeradorPlanilhas geradorPlanilhas = new GeradorPlanilhas();
			MemoryStream stream = geradorPlanilhas.CreateBasicWorkbook(true, lista);

			try
			{
				byte[] excelData = stream.ToArray();

				HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
				var stream2 = new MemoryStream(excelData);
				result.Content = new StreamContent(stream2);
				result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
				{
					FileName = "NomenclaturaProdutos.xlsx"
				};
				return result;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError);
			}
		}
	}
}
