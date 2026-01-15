<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="CaceiWeb.Login" %>

    <!DOCTYPE html>
    <html lang="es">

    <head runat="server">
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Iniciar Sesión - CACEI Sistema</title>
        <link href="Content/Site.css" rel="stylesheet" type="text/css" />
    </head>

    <body class="login-page">
        <form id="form1" runat="server">
            <div class="login-container">
                <div class="login-card">
                    <div class="login-header">
                        <h1>📋 CACEI Sistema</h1>
                        <p>Sistema de Gestión de Academias</p>
                    </div>

                    <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-error" Visible="false">
                        <asp:Literal ID="litError" runat="server"></asp:Literal>
                    </asp:Panel>

                    <div class="form-group">
                        <label for="txtUsuario">Usuario o Correo</label>
                        <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control"
                            placeholder="Ingresa tu usuario o correo" />
                    </div>

                    <div class="form-group">
                        <label for="txtPassword">Contraseña</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"
                            placeholder="Ingresa tu contraseña" />
                    </div>

                    <asp:Button ID="btnLogin" runat="server" Text="Iniciar Sesión" CssClass="btn btn-primary btn-block"
                        OnClick="btnLogin_Click" />
                </div>

                <div class="login-footer">
                    <p>&copy; <%= DateTime.Now.Year %> - CACEI Sistema de Gestión</p>
                </div>
            </div>
        </form>
    </body>

    </html>