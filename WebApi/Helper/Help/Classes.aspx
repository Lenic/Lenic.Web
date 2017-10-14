<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Classes.aspx.cs" Inherits="Helper.Help.Classes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link type="text/css" href="../Content/css/jquery-ui.css" rel="stylesheet" />
    <style type="text/css">
        body {
            font-family: "Segoe UI", Helvetica, Verdana;
            font-size: 11px;
            margin: 50px;
        }

        #wrapper {
            width: 900px;
            margin: 0 auto;
        }

        h1 {
            font-weight: normal;
        }

        .section {
            font-size: 1.3em;
            line-height: 1.5;
            margin: 1em 0;
        }

        td {
            padding: 0px 5px;
        }
    </style>
</head>
<body>
    <div id="wrapper">
        <h1>动态运行时模块列表</h1>

        <%
            if (TargetType.IsGenericType)
            {
        %>
        <p class="section">不支持泛型类型的查看！</p>
        <%
            }
            else if (DataSource.IsCLRType)
            {
        %>
        <p class="section">.Net 框架类型请检索 MSDN，感谢您的配合，谢谢！</p>
        <%
            }
            else
            {
        %>
        <p class="section">描述信息：<%= Server.HtmlEncode(DataSource.Description) %></p>
        <table border="1" style="border-collapse: collapse; font-size: 1.3em; font-family: 'Segoe UI', Helvetica, Verdana; line-height: 1.5; width: 880px; margin: 0 auto">
            <tr style="background-color: #3399ff; color: #ffffff;">
                <th>属性名称</th>
                <th>属性类型</th>
                <th>描述信息</th>
            </tr>
            <%
                foreach (var item in DataSource.Properties)
                {
            %>
            <tr>
                <td><strong><%= Server.HtmlEncode(item.Name) %></strong></td>
                <%
                    if (item.IsCLRType)
                    {
                %>
                <td><%= Server.HtmlEncode(item.Type) %></td>
                <%
                    }
                    else
                    {
                %>
                <td><a href="<%= item.OriginalTypeIdentity %>"><strong><%= Server.HtmlEncode(item.Type) %></strong></a></td>
                <%
                    }
                %>
                <td><%= item.Description %></td>
            </tr>
            <%
                }
            %>
        </table>
        <%
            }

        %>
    </div>
</body>
</html>