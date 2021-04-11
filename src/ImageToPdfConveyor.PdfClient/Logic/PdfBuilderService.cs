﻿namespace ImageToPdfConveyor.PdfClient.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using EnsureThat;
    using ImageToPdfConveyor.ObjectModel;
    using ImageToPdfConveyor.ObjectModel.Contracts;
    using PdfSharpCore.Drawing;
    using PdfSharpCore.Fonts;
    using PdfSharpCore.Utils;

    using PdfClientPdfDocument = ImageToPdfConveyor.ObjectModel.PdfDocument;
    using PdfDocument = PdfSharpCore.Pdf.PdfDocument;

    public sealed class PdfBuilderService : IPdfBuilderService
    {
        public PdfDocumentInfo BuildDocument(string directoryPath, PdfClientPdfDocument document)
        {
            EnsureArg.IsNotNullOrEmpty(directoryPath, nameof(directoryPath));
            EnsureArg.IsNotNull(document, nameof(document));

            GlobalFontSettings.FontResolver = new FontResolver();

            var pdfDocument = new PdfDocument();

            foreach (var documentPage in document.Pages)
            {
                var page = pdfDocument.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                var gfx = XGraphics.FromPdfPage(page);
                XImage xImage = XImage.FromStream(() => documentPage.DataStream);

                gfx.DrawImage(xImage, 0, 0, page.Width.Point, page.Height.Point);
            }

            pdfDocument.Save($"{directoryPath}/{document.Title}.pdf");

            return new PdfDocumentInfo
            {
                Created = DateTime.UtcNow,
                FilePath = $"{directoryPath}/{document.Title}.pdf",
            };
        }
    }
}
