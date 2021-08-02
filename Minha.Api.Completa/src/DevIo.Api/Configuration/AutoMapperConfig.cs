using AutoMapper;
using DevIo.Api.ViewModels;
using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIo.Api.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            CreateMap<ProdutoViewModel, Produto>();
            CreateMap<ProdutoImageViewModel, Produto>().ReverseMap();

            CreateMap<Produto, ProdutoViewModel>()
                        .ForMember(dest => dest.NomeFornecedor,
                                   opt => opt.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}
