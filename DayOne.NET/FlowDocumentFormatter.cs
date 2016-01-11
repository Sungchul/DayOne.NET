using CommonMark;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DayOne.NET
{
    public class FlowDocumentFormatter
    {
        public static void BlocksToFlowDocument(CommonMark.Syntax.Block blocks, FlowDocument document)
        {
            BlockToDocumentBlock(blocks, document.Blocks);
        }

        private static Paragraph GetHeadingItem(int level)
        {
            switch (level) {
                case 1:
                    return new Paragraph() { FontSize = 24, FontWeight = FontWeights.Bold };

                case 2:
                    return new Paragraph() { FontSize = 18, FontWeight = FontWeights.Bold };

                case 3:
                    return new Paragraph() { FontSize = 14, FontStyle = FontStyles.Italic };

                default:
                    return new Paragraph();
            }
        }

        private static void BlockToDocumentBlock(CommonMark.Syntax.Block block, ListItemCollection collection)
        {
            if (block == null)
                return;

            switch (block.Tag) {
                case BlockTag.ListItem:
                    var listitemBlock = new ListItem();

                    collection.Add(listitemBlock);
                    BlockToDocumentBlock(block.FirstChild, listitemBlock.Blocks);
                    BlockToDocumentBlock(block.NextSibling, collection);
                    break;

                default:
                    throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
            }
        }

        private static void BlockToDocumentBlock(CommonMark.Syntax.Block block, BlockCollection collection)
        {
            if (block == null)
                return;

            switch (block.Tag) {
                case BlockTag.Document:
                    BlockToDocumentBlock(block.FirstChild, collection);
                    break;

                case BlockTag.Paragraph:
                    var paragraph = new Paragraph();

                    CommonMark.Syntax.Inline current = block.InlineContent;
                    while (current != null) {
                        var inline = InlineToDocumentInline(current);
                        if (inline != null)
                            paragraph.Inlines.Add(inline);

                        current = current.NextSibling;
                    }

                    collection.Add(paragraph);

                    BlockToDocumentBlock(block.NextSibling, collection);
                    break;

                case BlockTag.BlockQuote:
                    var quoteBlock = new Section() { Background = Brushes.LightGray, Margin = new Thickness(10) };
                    collection.Add(quoteBlock);

                    BlockToDocumentBlock(block.FirstChild, quoteBlock.Blocks);
                    BlockToDocumentBlock(block.NextSibling, collection);
                    break;

                case BlockTag.ListItem:
                    throw new InvalidOperationException();

                case BlockTag.List:
                    var listBlock = new List();
                    collection.Add(listBlock);
                    if (block.FirstChild != null) {
                        if (block.FirstChild.ListData.ListType == ListType.Ordered) {
                            listBlock.MarkerStyle = TextMarkerStyle.Decimal;
                            listBlock.StartIndex = block.FirstChild.ListData.Start;
                        }

                        BlockToDocumentBlock(block.FirstChild, listBlock.ListItems);
                    }


                    BlockToDocumentBlock(block.NextSibling, collection);
                    break;

                case BlockTag.AtxHeading:
                case BlockTag.SetextHeading:
                    var heading = GetHeadingItem(block.Heading.Level);


                    heading.Inlines.Add(InlineToDocumentInline(block.InlineContent));
                    collection.Add(heading);

                    BlockToDocumentBlock(block.NextSibling, collection);
                    break;

                case BlockTag.IndentedCode:
                case BlockTag.FencedCode:
                    var codeBlock = new Section() { Background = Brushes.LightGray, Margin = new Thickness(2) };
                    collection.Add(codeBlock);
                    var code = block.StringContent.TakeFromStart(block.StringContent.Length - 1);

                    codeBlock.Blocks.Add(new Paragraph(new Run(code) { FontFamily = new FontFamily("Courier New") }));

                    BlockToDocumentBlock(block.NextSibling, collection);
                    break;

                case BlockTag.HtmlBlock:
                    // cannot output source position for HTML blocks
                    //block.StringContent.WriteTo(writer);
                    break;

                case BlockTag.ThematicBreak:
                    var separator = new Rectangle();
                    separator.Stroke = new SolidColorBrush(Colors.LightGray);
                    separator.StrokeThickness = 3;
                    separator.Height = 2;
                    separator.Width = double.NaN;

                    var lineBlock = new BlockUIContainer(separator);
                    collection.Add(lineBlock);

                    BlockToDocumentBlock(block.NextSibling, collection);
                    break;

                case BlockTag.ReferenceDefinition:

                    break;

                default:
                    throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);

            }
        }

        private static System.Windows.Documents.Inline InlineToDocumentInline(CommonMark.Syntax.Inline inline)
        {
            if (inline == null)
                return null;

            switch (inline.Tag) {
                case InlineTag.String:
                    return new Run(inline.LiteralContent);

                case InlineTag.LineBreak:
                    return new LineBreak();

                case InlineTag.SoftBreak:
                    return new Run(" ");

                case InlineTag.Code:
                    return new Run(inline.LiteralContent) { FontFamily = new FontFamily("Courier New") };

                case InlineTag.RawHtml:
                    break;

                case InlineTag.Link:
                    var hyperlink = new Hyperlink();
                    if (Uri.IsWellFormedUriString(inline.TargetUrl, UriKind.Absolute)) {
                        hyperlink.NavigateUri = new Uri(inline.TargetUrl);
                        hyperlink.RequestNavigate += new RequestNavigateEventHandler(LinkRequestNavigate);
                    }

                    var childInline = InlineToDocumentInline(inline.FirstChild);
                    if (childInline != null)
                        hyperlink.Inlines.Add(childInline);

                    return hyperlink;

                case InlineTag.Image:
                    Uri uri = null;
                    if (Uri.IsWellFormedUriString(inline.TargetUrl, UriKind.Absolute)) {
                        uri = new Uri(inline.TargetUrl, UriKind.Absolute);
                    }
                    else {
                        uri = new Uri(inline.TargetUrl, UriKind.Relative);
                    }

                    var image = new Image();
                    image.Source = new BitmapImage(uri);
                    var uiContainer = new BlockUIContainer(image);

                    return new Floater(uiContainer);

                case InlineTag.Strong:
                    return new Bold(InlineToDocumentInline(inline.FirstChild));

                case InlineTag.Emphasis:
                    return new Italic(InlineToDocumentInline(inline.FirstChild));

                case InlineTag.Strikethrough:
                    var span = new Span(InlineToDocumentInline(inline.FirstChild));
                    span.TextDecorations.Add(new TextDecoration() { Location = TextDecorationLocation.Strikethrough });
                    return span;

                default:
                    throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
            }

            return null;
        }

        private static void LinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
