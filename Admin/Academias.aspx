<%@ Page Title="Academias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="Academias.aspx.cs" Inherits="CaceiWeb.Admin.Academias" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <h1>🏛️ Gestión de Academias</h1>
            <div class="header-actions">
                <a href="ImportarAcademias.aspx" class="btn btn-secondary">📁 Importar CSV</a>
            </div>
        </div>

        <asp:Panel ID="pnlMensaje" runat="server" Visible="false">
            <asp:Literal ID="litMensaje" runat="server"></asp:Literal>
        </asp:Panel>

        <!-- Formulario de Academia -->
        <div class="card">
            <h3 class="card-title">
                <asp:Literal ID="litFormTitulo" runat="server">Nueva Academia</asp:Literal>
            </h3>

            <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-error" Visible="false">
                <asp:Literal ID="litError" runat="server"></asp:Literal>
            </asp:Panel>

            <asp:HiddenField ID="hfId" runat="server" Value="" />

            <div class="form-row">
                <div class="form-group form-col">
                    <label for="txtNombre">Nombre de la Academia *</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                        placeholder="Ej: Ingeniería en Sistemas" MaxLength="100" />
                </div>
                <div class="form-group form-col">
                    <label for="txtClave">Clave *</label>
                    <asp:TextBox ID="txtClave" runat="server" CssClass="form-control" placeholder="Ej: ISC"
                        MaxLength="20" />
                </div>
            </div>

            <div class="form-actions">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary"
                    OnClick="btnGuardar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary"
                    OnClick="btnCancelar_Click" Visible="false" />
            </div>
        </div>

        <!-- Listado de Academias -->
        <div class="card">
            <h3 class="card-title">Academias Registradas</h3>

            <div class="table-container">
                <asp:Repeater ID="rptAcademias" runat="server" OnItemCommand="rptAcademias_ItemCommand">
                    <HeaderTemplate>
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>Nombre</th>
                                    <th>Clave</th>
                                    <th>Estado</th>
                                    <th>Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><strong>
                                    <%# Eval("Nombre") %>
                                </strong></td>
                            <td><span class="badge badge-info">
                                    <%# Eval("Clave") %>
                                </span></td>
                            <td>
                                <%# (bool)Eval("Activo") ? "<span class='badge badge-success'>Activa</span>"
                                    : "<span class='badge badge-danger'>Inactiva</span>" %>
                            </td>
                            <td class="actions">
                                <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar"
                                    CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-secondary">Editar
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnToggle" runat="server"
                                    CommandName='<%# (bool)Eval("Activo") ? "Desactivar" : "Activar" %>'
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass='<%# (bool)Eval("Activo") ? "btn btn-sm btn-danger" : "btn btn-sm btn-success" %>'
                                    OnClientClick="return confirm('¿Estás seguro?');">
                                    <%# (bool)Eval("Activo") ? "Desactivar" : "Activar" %>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlSinDatos" runat="server" Visible="false" CssClass="empty-state">
                    <p>No hay academias registradas.</p>
                </asp:Panel>
            </div>
        </div>
    </asp:Content>