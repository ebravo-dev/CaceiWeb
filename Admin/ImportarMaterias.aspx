<%@ Page Title="Importar Materias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="ImportarMaterias.aspx.cs" Inherits="CaceiWeb.Admin.ImportarMaterias" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <h1>📁 Importar Materias desde CSV</h1>
        </div>

        <div class="card">
            <h3 class="card-title">Formato del Archivo CSV</h3>
            <p>Crea una tabla en Excel con las siguientes columnas y guárdala como CSV:</p>

            <div class="table-container">
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>nombre</th>
                            <th>clave</th>
                            <th>academia</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Programación I</td>
                            <td>PROG1</td>
                            <td>ISC</td>
                        </tr>
                        <tr>
                            <td>Base de Datos</td>
                            <td>BD01</td>
                            <td>ISC</td>
                        </tr>
                        <tr>
                            <td>Cálculo Diferencial</td>
                            <td>CALC1</td>
                            <td>MAT</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="info-box">
                <strong>Instrucciones:</strong>
                <ul>
                    <li>Crea la tabla en Excel con las 3 columnas</li>
                    <li>Guarda como: <strong>Archivo → Guardar como → CSV (delimitado por comas)</strong></li>
                    <li><strong>academia</strong>: Usa la clave de la academia (debe existir previamente)</li>
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
                                    <th>Clave</th>
                                    <th>Academia</th>
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
                                <%# Eval("Clave") %>
                            </td>
                            <td>
                                <%# Eval("AcademiaClave") %>
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
                <asp:Button ID="btnImportar" runat="server" Text="Importar Materias Válidas" CssClass="btn btn-success"
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
            <a href="Materias.aspx" class="btn btn-primary">Ver Materias</a>
            <a href="ImportarMaterias.aspx" class="btn btn-secondary">Importar Más</a>
        </asp:Panel>
    </asp:Content>