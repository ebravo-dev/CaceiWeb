<%@ Page Title="Importar Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="ImportarUsuarios.aspx.cs" Inherits="CaceiWeb.Admin.ImportarUsuarios" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <h1>📁 Importar Usuarios desde CSV</h1>
        </div>

        <div class="card">
            <h3 class="card-title">Formato del Archivo CSV</h3>
            <p>Crea una tabla en Excel con las siguientes columnas y guárdala como CSV:</p>

            <div class="table-container">
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>nombre</th>
                            <th>correo</th>
                            <th>rol</th>
                            <th>academias</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Juan Pérez</td>
                            <td>juan@uat.edu.mx</td>
                            <td>profesor</td>
                            <td>ISC;IIA</td>
                        </tr>
                        <tr>
                            <td>María López</td>
                            <td>maria@uat.edu.mx</td>
                            <td>presidente</td>
                            <td>ISC</td>
                        </tr>
                        <tr>
                            <td>Pedro García</td>
                            <td>pedro@uat.edu.mx</td>
                            <td>profesor</td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="info-box">
                <strong>Instrucciones:</strong>
                <ul>
                    <li>Crea la tabla en Excel con las 4 columnas</li>
                    <li>Guarda como: <strong>Archivo → Guardar como → CSV (delimitado por comas)</strong></li>
                    <li><strong>rol</strong>: Usa exactamente: admin, presidente, o profesor</li>
                    <li><strong>academias</strong>: Si tiene varias, sepáralas con punto y coma (;)</li>
                    <li>La contraseña por defecto será: <code>cacei2024</code></li>
                </ul>
            </div>
        </div>

        <asp:Panel ID="pnlUpload" runat="server" CssClass="card">
            <h3 class="card-title">Subir Archivo</h3>

            <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-error" Visible="false">
                <asp:Literal ID="litError" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="form-group">
                <label>Archivo CSV</label>
                <asp:FileUpload ID="fuArchivo" runat="server" CssClass="form-control" accept=".csv,.txt" />
            </div>

            <asp:Button ID="btnAnalizar" runat="server" Text="Analizar Archivo" CssClass="btn btn-primary"
                OnClick="btnAnalizar_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlPreview" runat="server" CssClass="card" Visible="false">
            <h3 class="card-title">Vista Previa</h3>

            <asp:Panel ID="pnlWarnings" runat="server" CssClass="alert alert-warning" Visible="false">
                <strong>Advertencias:</strong>
                <asp:Literal ID="litWarnings" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="table-container">
                <asp:Repeater ID="rptPreview" runat="server">
                    <HeaderTemplate>
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Nombre</th>
                                    <th>Correo</th>
                                    <th>Rol</th>
                                    <th>Academias</th>
                                    <th>Estado</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class='<%# Eval("EsValido").ToString() == "True" ? "" : "row-error" %>'>
                            <td>
                                <%# Eval("Linea") %>
                            </td>
                            <td>
                                <%# Eval("Nombre") %>
                            </td>
                            <td>
                                <%# Eval("Correo") %>
                            </td>
                            <td>
                                <%# Eval("Rol") %>
                            </td>
                            <td>
                                <%# Eval("Academias") %>
                            </td>
                            <td>
                                <%# Eval("EsValido").ToString()=="True" ? "<span class='badge badge-success'>OK</span>"
                                    : "<span class='badge badge-danger'>" + Eval("Error") + "</span>" %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <div class="preview-summary">
                <p>
                    <strong>Total:</strong>
                    <asp:Literal ID="litTotal" runat="server"></asp:Literal> registros |
                    <strong>Válidos:</strong>
                    <asp:Literal ID="litValidos" runat="server"></asp:Literal> |
                    <strong>Con errores:</strong>
                    <asp:Literal ID="litErrores" runat="server"></asp:Literal>
                </p>
            </div>

            <div class="form-actions">
                <asp:Button ID="btnImportar" runat="server" Text="Importar Usuarios Válidos" CssClass="btn btn-success"
                    OnClick="btnImportar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary"
                    OnClick="btnCancelar_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlResultado" runat="server" CssClass="card" Visible="false">
            <h3 class="card-title">Resultado de Importación</h3>
            <div class="alert alert-success">
                <asp:Literal ID="litResultado" runat="server"></asp:Literal>
            </div>
            <a href="Usuarios.aspx" class="btn btn-primary">Ver Usuarios</a>
            <a href="ImportarUsuarios.aspx" class="btn btn-secondary">Importar Más</a>
        </asp:Panel>
    </asp:Content>