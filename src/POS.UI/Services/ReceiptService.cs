using System;
using System.IO;
using System.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using POS.Bridge.DataTransferObjects;

namespace POS.UI.Services
{
    public class ReceiptService
    {
        public void GenerateReceipt(OrderDTO order, double cashReceived, double change, string filePath)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Receipt #{order.Id}";

            // Create an empty page
            PdfPage page = document.AddPage();
            
            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create fonts
            XFont fontTitle = new XFont("Verdana", 20, XFontStyleEx.Bold);
            XFont fontHeader = new XFont("Verdana", 12, XFontStyleEx.Bold);
            XFont fontRegular = new XFont("Verdana", 10, XFontStyleEx.Regular);
            XFont fontSmall = new XFont("Verdana", 8, XFontStyleEx.Regular);

            int yPoint = 40;
            int margin = 40;
            int pageWidth = (int)page.Width;
            int contentWidth = pageWidth - (margin * 2);

            // 1. Header (Shop Name)
            gfx.DrawString("POS Restaurant", fontTitle, XBrushes.Black,
                new XRect(0, yPoint, pageWidth, 30), XStringFormats.TopCenter);
            yPoint += 40;

            gfx.DrawString("123 Food Street, Tasty City", fontRegular, XBrushes.Gray,
                new XRect(0, yPoint, pageWidth, 20), XStringFormats.TopCenter);
            yPoint += 20;
            
            gfx.DrawString($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}", fontSmall, XBrushes.Black,
                new XRect(0, yPoint, pageWidth, 20), XStringFormats.TopCenter);
            yPoint += 40;

            // 2. Order Info
            // Draw Line
            gfx.DrawLine(XPens.Black, margin, yPoint, pageWidth - margin, yPoint);
            yPoint += 10;

            gfx.DrawString($"Order ID: #{order.Id}", fontHeader, XBrushes.Black, margin, yPoint);
            yPoint += 20;

            string typeText = order.Type == OrderTypeDTO.DineIn ? $"Table: {order.TableNumber}" : "Parcel";
            if (order.Type == OrderTypeDTO.Parcel && order.Provider != ParcelProviderDTO.None)
            {
                typeText += $" ({order.Provider})";
            }
            
            gfx.DrawString($"Type: {typeText}", fontRegular, XBrushes.Black, margin, yPoint);
            yPoint += 30;

            // 3. Items Header
            gfx.DrawLine(XPens.Black, margin, yPoint, pageWidth - margin, yPoint);
            yPoint += 5;
            
            // Columns: Item (50%), Qty (15%), Price (35%)
            int col1X = margin;
            int col2X = margin + (int)(contentWidth * 0.6);
            int col3X = margin + (int)(contentWidth * 0.85);

            gfx.DrawString("Item", fontHeader, XBrushes.Black, col1X, yPoint);
            gfx.DrawString("Qty", fontHeader, XBrushes.Black, col2X, yPoint);
            gfx.DrawString("Total", fontHeader, XBrushes.Black, col3X, yPoint);
            
            yPoint += 20;
            gfx.DrawLine(XPens.Black, margin, yPoint, pageWidth - margin, yPoint);
            yPoint += 10;

            // 4. Items List
            foreach (var item in order.Items)
            {
                gfx.DrawString(item.MenuName, fontRegular, XBrushes.Black, col1X, yPoint);
                gfx.DrawString(item.Quantity.ToString(), fontRegular, XBrushes.Black, col2X, yPoint);
                gfx.DrawString($"${item.Subtotal:F2}", fontRegular, XBrushes.Black, col3X, yPoint);
                
                yPoint += 20;
            }

            yPoint += 10;
            gfx.DrawLine(XPens.Black, margin, yPoint, pageWidth - margin, yPoint);
            yPoint += 10;

            // 5. Totals
            int labelX = margin + (int)(contentWidth * 0.6);
            int valueX = margin + (int)(contentWidth * 0.85);

            gfx.DrawString("Subtotal:", fontRegular, XBrushes.Black, labelX, yPoint);
            gfx.DrawString($"${order.TotalAmount:F2}", fontRegular, XBrushes.Black, valueX, yPoint);
            yPoint += 20;

            // Assuming tax is included or calculated separately, standardizing here based on CheckoutDialog logic 
            // CheckoutDialog shows Tax (0%), so we mirror that or just show Total if Tax is 0.
            // But let's stay consistent.
            gfx.DrawString("Tax (0%):", fontRegular, XBrushes.Black, labelX, yPoint);
            gfx.DrawString($"$0.00", fontRegular, XBrushes.Black, valueX, yPoint);
            yPoint += 20;

            gfx.DrawString("Total:", fontHeader, XBrushes.Black, labelX, yPoint);
            gfx.DrawString($"${order.TotalAmount:F2}", fontHeader, XBrushes.Black, valueX, yPoint);
            yPoint += 30;

            gfx.DrawString("Cash:", fontRegular, XBrushes.Black, labelX, yPoint);
            gfx.DrawString($"${cashReceived:F2}", fontRegular, XBrushes.Black, valueX, yPoint);
            yPoint += 20;

            gfx.DrawString("Change:", fontRegular, XBrushes.Black, labelX, yPoint);
            gfx.DrawString($"${change:F2}", fontRegular, XBrushes.Black, valueX, yPoint);
            yPoint += 40;

            // 6. Footer
            gfx.DrawLine(XPens.Black, margin, yPoint, pageWidth - margin, yPoint);
            yPoint += 10;
            gfx.DrawString("Thank you for your business!", fontRegular, XBrushes.Black, 
                new XRect(0, yPoint, pageWidth, 20), XStringFormats.TopCenter);

            // Save the document...
            document.Save(filePath);
        }
    }
}
