using BulkyBook.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BulkyBookWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tagBuilder = new TagBuilder("div");

            for (var i = 1; i <= PageModel.TotalPage; i++)
            {
                var builder = new TagBuilder("a");
                var url = PageModel.UrlParameter.Replace(":", i.ToString());
                builder.Attributes["href"] = url;

                if (PageClassesEnabled)
                {
                    builder.AddCssClass(PageClass);
                    builder.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }

                builder.InnerHtml.Append(i.ToString());
                tagBuilder.InnerHtml.AppendHtml(builder);
            }

            output.Content.AppendHtml(tagBuilder.InnerHtml);
        }
    } 
}