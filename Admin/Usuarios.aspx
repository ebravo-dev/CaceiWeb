<%@ Page Title="Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="Usuarios.aspx.cs" Inherits="CaceiWeb.Admin.Usuarios" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <h1>👥 Gestión de Usuarios</h1>
            <div class="header-actions">
                <a href="ImportarUsuarios.aspx" class="btn btn-secondary">📁 Importar CSV</a>
                <a href="UsuarioForm.aspx" class="btn btn-primary">+ Nuevo Usuario</a>
            </div>
        </div>

        <asp:Panel ID="pnlMensaje" runat="server" Visible="false">
            <asp:Literal ID="litMensaje" runat="server"></asp:Literal>
        </asp:Panel>

        <div class="card">
            <div class="table-container">
                <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                    <HeaderTemplate>
                        <table class="data-table">
                            <thead>
                                <tr>
                                    <th>Nombre</th>
                                    <th>Correo</th>
                                    <th>Rol</th>
                                    <th>Academias</th>
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
                            <td>
                                <%# Eval("Correo") %>
                            </td>
                            <td>
                                <span class="badge badge-info">
                                    <%# Eval("RolNombre") %>
                                </span>
                            </td>
                            <td>
                                <%# GetAcademiasString((CaceiWeb.Models.Usuario)Container.DataItem) %>
                            </td>
                            <td>
                                <%# (bool)Eval("Activo") ? "<span class='badge badge-success'>Activo</span>"
                                    : "<span class='badge badge-danger'>Inactivo</span>" %>
                            </td>
                            <td class="actions">
                                <a href='UsuarioForm.aspx?id=<%# Eval("Id") %>'
                                    class="btn btn-sm btn-secondary">Editar</a>
                                <asp:LinkButton ID="btnToggle" runat="server"
                                    CommandName='<%# (bool)Eval("Activo") ? "Desactivar" : "Activar" %>'
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass='<%# (bool)Eval("Activo") ? "btn btn-sm btn-danger" : "btn btn-sm btn-success" %>'>
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
                    <p>No hay usuarios registrados.</p>
                    <a href="UsuarioForm.aspx" class="btn btn-primary">Crear primer usuario</a>
                </asp:Panel>
            </div>
        </div>
    </asp:Content>