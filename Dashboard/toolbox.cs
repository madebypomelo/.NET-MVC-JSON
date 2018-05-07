using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class toolbox
{
    //Tools for querystrings
    public static string GetQuerystring(string Parameter)
    {
        var QueryString = HttpContext.Current.Request.QueryString[Parameter];
        if (QueryString != null)
        {
            return HttpContext.Current.Request.QueryString[Parameter];
        }

        return "null";
    }
    public static string ModifyQuerystring(string Parameter, string Value)
    {
        var url = HttpUtility.ParseQueryString(HttpContext.Current.Request.QueryString.ToString());
        url.Set(Parameter, Value);
        return HttpContext.Current.Request.Url.AbsolutePath + "?" + url.ToString();
    }
}