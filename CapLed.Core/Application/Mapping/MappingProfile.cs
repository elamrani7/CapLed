using StockManager.Core.Domain.Enums;
using AutoMapper;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Entities.Stock;
using StockManager.Core.Domain.Entities.Commercial;
using StockManager.Core.Domain.Entities.Catalogue;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.DTOs.Stock;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.DTOs.Documents;
using StockManager.Core.Application.DTOs.Catalogue;

namespace StockManager.Core.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category & Famille Mappings
        CreateMap<Famille, FamilleDto>().ReverseMap();
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.FamilleLibelle, opt => opt.MapFrom(src => src.Famille != null ? src.Famille.Libelle : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Famille, opt => opt.Ignore());

        // Equipment Mappings (Back-office)
        CreateMap<Equipment, EquipmentListItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Label : string.Empty))
            .ForMember(dest => dest.AlertLevel, opt => opt.MapFrom(src => StockAlertHelper.GetAlertLevel(src.Quantity)))
            .ForMember(dest => dest.TypeGestionStock, opt => opt.MapFrom(src => src.Category != null ? src.Category.TypeGestionStock : string.Empty))
            .ForMember(dest => dest.MinThreshold, opt => opt.MapFrom(src => src.MinThreshold))
            .ForMember(dest => dest.PrixVente, opt => opt.MapFrom(src => src.PrixVente));

        CreateMap<Equipment, EquipmentReadDto>()
            .IncludeBase<Equipment, EquipmentListItemDto>();

        CreateMap<EquipmentCreateDto, Equipment>();
        CreateMap<EquipmentUpdateDto, Equipment>();

        // Photos
        CreateMap<Photo, PhotoDto>();

        // Equipment Mappings (Front-office Catalog)
        CreateMap<Equipment, EquipmentCatalogItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Label : string.Empty))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.MainPhotoUrl, opt => opt.MapFrom(src => 
                src.Photos.Where(p => p.IsPrimary).Select(p => p.Url).FirstOrDefault() ?? 
                src.Photos.Select(p => p.Url).FirstOrDefault()));

        CreateMap<Equipment, EquipmentCatalogDetailDto>()
            .IncludeBase<Equipment, EquipmentCatalogItemDto>()
            .ForMember(dest => dest.ChampsSpecifiques, opt => opt.MapFrom(src => src.ChampsSpecifiques.OrderBy(v => v.ChampSpecifique.Ordre)));

        // StockMovement Mappings
        CreateMap<StockMovement, StockMovementReadDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.Name : string.Empty))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Remarks));

        CreateMap<StockMovementCreateDto, StockMovement>()
            .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Date));

        // Step 2 + 3: MouvementStockReadDto mapping
        CreateMap<StockMovement, MouvementStockReadDto>()
            .ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.EquipmentId))
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.Name : string.Empty))
            .ForMember(dest => dest.TypeMouvement, opt => opt.MapFrom(src => src.TypeMouvement))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.DepotSourceId, opt => opt.MapFrom(src => src.DepotSourceId))
            .ForMember(dest => dest.DepotSourceNom, opt => opt.MapFrom(src => src.DepotSource != null ? src.DepotSource.Nom : string.Empty))
            .ForMember(dest => dest.DepotDestinationId, opt => opt.MapFrom(src => src.DepotDestinationId))
            .ForMember(dest => dest.DepotDestinationNom, opt => opt.MapFrom(src => src.DepotDestination != null ? src.DepotDestination.Nom : string.Empty))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            .ForMember(dest => dest.LotId, opt => opt.MapFrom(src => src.LotId))
            .ForMember(dest => dest.NumeroLot, opt => opt.MapFrom(src => src.Lot != null ? src.Lot.NumeroLot : null))
            .ForMember(dest => dest.NumeroSeries, opt => opt.Ignore()); // populated manually by controller if needed

        // Step 4A: Commercial mappings
        CreateMap<Client, ClientReadDto>();
        CreateMap<CreateClientDto, Client>();

        CreateMap<Lead, LeadReadDto>()
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
            .ForMember(dest => dest.CommercialNom, opt => opt.MapFrom(src => src.Commercial != null ? src.Commercial.FullName : null))
            .ForMember(dest => dest.Lignes, opt => opt.MapFrom(src => src.Lignes));

        CreateMap<LigneLead, LigneLeadReadDto>()
            .ForMember(dest => dest.ArticleRef, opt => opt.MapFrom(src => src.Article != null ? src.Article.Reference : string.Empty))
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Article != null ? src.Article.Name : string.Empty));

        // Step 4B: Orders & Deliveries
        CreateMap<BonCommande, BonCommandeReadDto>()
            .ForMember(dest => dest.ClientNom, opt => opt.MapFrom(src => src.Client.Nom))
            .ForMember(dest => dest.Lignes, opt => opt.MapFrom(src => src.Lignes));

        CreateMap<CreateBonCommandeDto, BonCommande>();
        
        CreateMap<LigneBC, LigneBCReadDto>()
            .ForMember(dest => dest.ArticleRef, opt => opt.MapFrom(src => src.Article.Reference))
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Article.Name));
            
        CreateMap<CreateLigneBCDto, LigneBC>();

        CreateMap<BonLivraison, BonLivraisonReadDto>()
            .ForMember(dest => dest.ClientNom, opt => opt.MapFrom(src => src.Client.Nom))
            .ForMember(dest => dest.NumeroBC, opt => opt.MapFrom(src => src.BonCommande != null ? src.BonCommande.NumeroBC : null))
            .ForMember(dest => dest.Lignes, opt => opt.MapFrom(src => src.Lignes));

        CreateMap<CreateBonLivraisonDto, BonLivraison>();

        CreateMap<LigneBL, LigneBLReadDto>()
            .ForMember(dest => dest.ArticleRef, opt => opt.MapFrom(src => src.Article.Reference))
            .ForMember(dest => dest.ArticleName, opt => opt.MapFrom(src => src.Article.Name));

        CreateMap<CreateLigneBLDto, LigneBL>();

        // Step 5: PDF Generation
        CreateMap<Lead, DevisPdfDto>()
            .ForMember(dest => dest.NumeroDevis, opt => opt.MapFrom(src => src.NumeroDevis))
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => src.DateSoumission))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Nom))
            .ForMember(dest => dest.ClientEmail, opt => opt.MapFrom(src => src.Client.Email))
            .ForMember(dest => dest.ClientTelephone, opt => opt.MapFrom(src => src.Client.Telephone))
            .ForMember(dest => dest.ClientSociete, opt => opt.MapFrom(src => src.Client.Societe))
            .ForMember(dest => dest.ClientAdresse, opt => opt.MapFrom(src => src.Client.Adresse))
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lignes))
            .ForMember(dest => dest.TotalHT, opt => opt.MapFrom(src => 0)); // Price logic not implemented in domain yet, using 0

        CreateMap<LigneLead, DocumentLinePdfDto>()
            .ForMember(dest => dest.ArticleRef, opt => opt.MapFrom(src => src.Article.Reference))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Article.Name))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.QuantiteDemandee))
            .ForMember(dest => dest.PrixUnitaire, opt => opt.MapFrom(src => 0));

        CreateMap<BonCommande, BonCommandePdfDto>()
            .ForMember(dest => dest.NumeroBC, opt => opt.MapFrom(src => src.NumeroBC))
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(src => src.DateCommande))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Nom))
            .ForMember(dest => dest.ClientEmail, opt => opt.MapFrom(src => src.Client.Email))
            .ForMember(dest => dest.ClientTelephone, opt => opt.MapFrom(src => src.Client.Telephone))
            .ForMember(dest => dest.ClientSociete, opt => opt.MapFrom(src => src.Client.Societe))
            .ForMember(dest => dest.ClientAdresse, opt => opt.MapFrom(src => src.Client.Adresse))
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lignes))
            .ForMember(dest => dest.TotalHT, opt => opt.MapFrom(src => 0));

        CreateMap<LigneBC, DocumentLinePdfDto>()
            .ForMember(dest => dest.ArticleRef, opt => opt.MapFrom(src => src.Article.Reference))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Article.Name))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.QuantiteCommandee))
            .ForMember(dest => dest.PrixUnitaire, opt => opt.MapFrom(src => 0));

        CreateMap<BonLivraison, BonLivraisonPdfDto>()
            .ForMember(dest => dest.NumeroBL, opt => opt.MapFrom(src => src.NumeroBL))
            .ForMember(dest => dest.NumeroBC, opt => opt.MapFrom(src => src.BonCommande != null ? src.BonCommande.NumeroBC : null))
            .ForMember(dest => dest.DateLivraison, opt => opt.MapFrom(src => src.DateLivraison))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Nom))
            .ForMember(dest => dest.ClientEmail, opt => opt.MapFrom(src => src.Client.Email))
            .ForMember(dest => dest.ClientTelephone, opt => opt.MapFrom(src => src.Client.Telephone))
            .ForMember(dest => dest.AdresseLivraison, opt => opt.MapFrom(src => src.AdresseLivraison))
            .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lignes));

        CreateMap<LigneBL, DocumentLinePdfDto>()
            .ForMember(dest => dest.ArticleRef, opt => opt.MapFrom(src => src.Article.Reference))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Article.Name))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.QuantiteLivree));

        // Step 6: EAV Mappings
        CreateMap<CreateChampSpecifiqueDto, ChampSpecifique>();
        CreateMap<ChampSpecifique, ChampSpecifiqueReadDto>();
        CreateMap<ArticleChampValeur, ArticleChampValeurDto>()
            .ForMember(dest => dest.NomChamp, opt => opt.MapFrom(src => src.ChampSpecifique.NomChamp))
            .ForMember(dest => dest.TypeDonnee, opt => opt.MapFrom(src => src.ChampSpecifique.TypeDonnee));

        // Step 7: Article Condition Details
        CreateMap<ArticleEtatDetail, ArticleEtatDetailDto>();
        CreateMap<CreateOrUpdateArticleEtatDetailDto, ArticleEtatDetail>();

        // Step 8: Advanced Public Catalogue
        CreateMap<ArticleChampValeur, PublicSpecDto>()
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.ChampSpecifique.NomChamp))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ChampSpecifique.TypeDonnee))
            .ForMember(dest => dest.Valeur, opt => opt.MapFrom(src => src.Valeur));

        CreateMap<Equipment, PublicArticleListItemDto>()
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.FamilleNom, opt => opt.MapFrom(src => src.Category.Famille != null ? src.Category.Famille.Libelle : ""))
            .ForMember(dest => dest.CategorieNom, opt => opt.MapFrom(src => src.Category.Label))
            .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.Condition.ToString()))
            .ForMember(dest => dest.UrlImagePrincipale, opt => opt.MapFrom(src => 
                src.Photos.FirstOrDefault(p => p.IsPrimary) != null ? src.Photos.FirstOrDefault(p => p.IsPrimary)!.Url : 
                (src.Photos.FirstOrDefault() != null ? src.Photos.FirstOrDefault()!.Url : null)))
            .ForMember(dest => dest.BadgeDisponibilite, opt => opt.MapFrom(src => 

                src.DisponibiliteSite == "EN_STOCK" ? "En stock" :
                src.DisponibiliteSite == "STOCK_LIMITE" ? "Stock limité" :
                src.DisponibiliteSite == "SUR_COMMANDE" ? "Sur commande" : "Indisponible"))
            .ForMember(dest => dest.BadgeCondition, opt => opt.MapFrom(src => 
                src.Condition.ToString() == "OCCASION" ? "Occasion" + (src.EtatDetail != null && src.EtatDetail.GradeVisuel != null ? " (Grade " + src.EtatDetail.GradeVisuel + ")" : "") :
                src.Condition.ToString() == "RECONDITIONNE" ? "Reconditionné" + (src.EtatDetail != null && src.EtatDetail.GradeVisuel != null ? " (Grade " + src.EtatDetail.GradeVisuel + ")" : "") : 
                src.Condition.ToString() == "EN_REPARATION" ? "En Réparation" : 
                src.Condition.ToString() == "ENDOMMAGE" ? "Endommagé" : 
                "Neuf"));

        CreateMap<Equipment, PublicArticleDetailDto>()
            .ForMember(dest => dest.Nom, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.Condition.ToString()))
            .ForMember(dest => dest.DisponibiliteBadge, opt => opt.MapFrom(src => 
                src.DisponibiliteSite == "EN_STOCK" ? "En stock" :
                src.DisponibiliteSite == "STOCK_LIMITE" ? "Stock limité" :
                src.DisponibiliteSite == "SUR_COMMANDE" ? "Sur commande" : "Indisponible"))
            .ForMember(dest => dest.ConditionBadge, opt => opt.MapFrom(src => 
                src.Condition.ToString() == "OCCASION" ? "Occasion" + (src.EtatDetail != null && src.EtatDetail.GradeVisuel != null ? " (Grade " + src.EtatDetail.GradeVisuel + ")" : "") :
                src.Condition.ToString() == "RECONDITIONNE" ? "Reconditionné" + (src.EtatDetail != null && src.EtatDetail.GradeVisuel != null ? " (Grade " + src.EtatDetail.GradeVisuel + ")" : "") : 
                src.Condition.ToString() == "EN_REPARATION" ? "En Réparation" : 
                src.Condition.ToString() == "ENDOMMAGE" ? "Endommagé" : 
                "Neuf"))
            .ForMember(dest => dest.CaracteristiquesPrincipales, opt => opt.MapFrom(src => src.ChampsSpecifiques.OrderBy(v => v.ChampSpecifique.Ordre)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Photos.OrderByDescending(p => p.IsPrimary).Select(p => p.Url).ToList()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
            .ForMember(dest => dest.ArticlesSimilaires, opt => opt.Ignore()); // Mapped manually in service




        // User Mappings
        CreateMap<User, UserReadDto>();
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();

        // Photo Mapping
        CreateMap<Photo, PhotoDto>().ReverseMap();

        // Alert Mappings
        // Note: Logic for AlertReadDto status should be handled in the service/mapping
        CreateMap<Equipment, AlertReadDto>()
            .ForMember(dest => dest.EquipmentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CurrentQuantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Threshold, opt => opt.MapFrom(src => src.MinThreshold))
            .ForMember(dest => dest.AlertLevel, opt => opt.MapFrom(src => StockAlertHelper.GetAlertLevel(src.Quantity)));

        // ContactRequest Mappings
        CreateMap<ContactRequest, ContactRequestReadDto>()
            .ForMember(dest => dest.RelatedEquipmentNames, opt => opt.MapFrom(src => 
                (src.Equipment != null) ? new List<string> { src.Equipment.Name } : new List<string>()));

        CreateMap<ContactRequestCreateDto, ContactRequest>()
            .ForMember(dest => dest.EquipmentId, opt => opt.MapFrom(src => src.EquipmentIds.Any() ? (int?)src.EquipmentIds.First() : null))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.VisitorName))
            .ForMember(dest => dest.SenderEmail, opt => opt.MapFrom(src => src.Email));
    }
}
