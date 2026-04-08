using System;
using AutoMapper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using StockManager.Core.Application.DTOs.Documents;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Commercial;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManager.Infrastructure.Services;

public class DocumentPdfService : IDocumentPdfService
{
    private readonly ILeadRepository _leadRepo;
    private readonly IBonCommandeRepository _bcRepo;
    private readonly IBonLivraisonRepository _blRepo;
    private readonly IMapper _mapper;

    public DocumentPdfService(
        ILeadRepository leadRepo,
        IBonCommandeRepository bcRepo,
        IBonLivraisonRepository blRepo,
        IMapper mapper)
    {
        _leadRepo = leadRepo;
        _bcRepo = bcRepo;
        _blRepo = blRepo;
        _mapper = mapper;
        
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateDevisPdfAsync(int leadId)
    {
        var lead = await _leadRepo.GetByIdAsync(leadId);
        if (lead == null) return Array.Empty<byte>();

        var dto = _mapper.Map<DevisPdfDto>(lead);
        return GenerateDocument("DEVIS", dto.NumeroDevis, dto.DateCreation, dto.ClientName, dto.ClientAdresse, dto.Lines, "HT");
    }

    public async Task<byte[]> GenerateBonCommandePdfAsync(int bcId)
    {
        var bc = await _bcRepo.GetByIdAsync(bcId);
        if (bc == null) return Array.Empty<byte>();

        var dto = _mapper.Map<BonCommandePdfDto>(bc);
        return GenerateDocument("BON DE COMMANDE", dto.NumeroBC, dto.DateCreation, dto.ClientName, dto.ClientAdresse, dto.Lines, "HT");
    }

    public async Task<byte[]> GenerateBonLivraisonPdfAsync(int blId)
    {
        var bl = await _blRepo.GetByIdAsync(blId);
        if (bl == null) return Array.Empty<byte>();

        var dto = _mapper.Map<BonLivraisonPdfDto>(bl);
        
        // Custom logic for BL as it has delivery address and no prices usually (or just quantities)
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                ComposeHeader(page, "BON DE LIVRAISON", dto.NumeroBL, dto.DateLivraison);
                
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Client").SemiBold();
                            c.Item().Text(dto.ClientName);
                            c.Item().Text(dto.AdresseLivraison ?? "N/A");
                        });
                    });

                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("Article");
                            header.Cell().Element(CellStyle).AlignRight().Text("Qté Livrée");

                            static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                        });

                        foreach (var item in dto.Lines)
                        {
                            table.Cell().Element(CellStyle).Text((dto.Lines.IndexOf(item) + 1).ToString());
                            table.Cell().Element(CellStyle).Text(item.Description);
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Quantite.ToString());

                            static IContainer CellStyle(IContainer container) => container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                        }
                    });
                    
                    col.Item().PaddingTop(30).Text("Signature Client :").Italic();
                    col.Item().Height(50).Border(1).BorderColor(Colors.Grey.Lighten2);
                });

                ComposeFooter(page);
            });
        }).GeneratePdf();
    }

    private byte[] GenerateDocument(string title, string number, DateTime date, string clientName, string? address, List<DocumentLinePdfDto> lines, string totalLabel)
    {
        decimal total = lines.Sum(x => x.TotalLigne);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                ComposeHeader(page, title, number, date);
                
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Destinataire").SemiBold();
                            c.Item().Text(clientName);
                            c.Item().Text(address ?? "N/A");
                        });
                    });

                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("Désignation");
                            header.Cell().Element(CellStyle).AlignRight().Text("P.U.");
                            header.Cell().Element(CellStyle).AlignRight().Text("Qté");
                            header.Cell().Element(CellStyle).AlignRight().Text("Total");

                            static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                        });

                        foreach (var item in lines)
                        {
                            table.Cell().Element(CellStyle).Text((lines.IndexOf(item) + 1).ToString());
                            table.Cell().Element(CellStyle).Text($"{item.ArticleRef} - {item.Description}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{item.PrixUnitaire:N2} €");
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Quantite.ToString());
                            table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalLigne:N2} €");

                            static IContainer CellStyle(IContainer container) => container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                        }
                    });

                    col.Item().AlignRight().PaddingTop(20).Text(t =>
                    {
                        t.Span($"TOTAL {totalLabel} : ").FontSize(14);
                        t.Span($"{total:N2} €").FontSize(14).SemiBold().FontColor("#0056A6");
                    });
                });

                ComposeFooter(page);
            });
        }).GeneratePdf();
    }

    private void ComposeHeader(PageDescriptor page, string title, string number, DateTime date)
    {
        page.Header().Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("CAPLED ERP").FontSize(24).SemiBold().FontColor("#0056A6");
                col.Item().Text("Solutions Industrielles & Stockage");
                col.Item().Text("123 Rue de l'Innovation, 75000 Paris");
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text(title).FontSize(20).ExtraBold();
                col.Item().Text($"N° {number}");
                col.Item().Text($"Date : {date:dd/MM/yyyy}");
            });
        });
    }

    private void ComposeFooter(PageDescriptor page)
    {
        page.Footer().AlignCenter().Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            col.Item().PaddingTop(5).Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
            col.Item().Text("Siret: 123 456 789 00010 - TVA Intra: FR88123456789").FontSize(9).FontColor(Colors.Grey.Medium);
        });
    }
}
