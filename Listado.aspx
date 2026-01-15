<%@ Page Title="Listado" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Listado.aspx.cs"
    Inherits="CaceiWeb.Listado" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <div class="card">
            <h2 class="card-title">📋 Listado de Registros</h2>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false">
                <asp:Label ID="lblMensaje" runat="server" CssClass="alert"></asp:Label>
            </asp:Panel>

            <div class="table-container">
                <asp:GridView ID="gvRegistros" runat="server" AutoGenerateColumns="false" CssClass="data-table"
                    EmptyDataText="No hay registros para mostrar." OnRowCommand="gvRegistros_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="Matricula" HeaderText="Matrícula" />
                        <asp:BoundField DataField="Carrera" HeaderText="Carrera" />
                        <asp:BoundField DataField="Semestre" HeaderText="Semestre" />
                        <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha"
                            DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar"
                                    CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-danger"
                                    OnClientClick="return confirm('¿Estás seguro de eliminar este registro?');"
                                    style="padding: 0.5rem 1rem; font-size: 0.875rem;">
                                    🗑️ Eliminar
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div style="margin-top: 2rem;">
                <a href="Captura.aspx" class="btn btn-primary">➕ Nuevo Registro</a>
                <asp:Button ID="btnRefrescar" runat="server" Text="🔄 Refrescar" CssClass="btn btn-success"
                    OnClick="btnRefrescar_Click" style="margin-left: 1rem;" />
            </div>
        </div>

    </asp:Content>