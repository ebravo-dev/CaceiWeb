<%@ Page Title="Captura" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Captura.aspx.cs"
    Inherits="CaceiWeb.Captura" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <div class="card">
            <h2 class="card-title">📝 Formulario de Captura</h2>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false">
                <asp:Label ID="lblMensaje" runat="server" CssClass="alert"></asp:Label>
            </asp:Panel>

            <div class="form-group">
                <label for="txtNombre">Nombre Completo *</label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                    placeholder="Ingresa el nombre completo" required="required"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="txtMatricula">Matrícula</label>
                <asp:TextBox ID="txtMatricula" runat="server" CssClass="form-control" placeholder="Ej: 2021001234">
                </asp:TextBox>
            </div>

            <div class="form-group">
                <label for="txtCarrera">Carrera</label>
                <asp:TextBox ID="txtCarrera" runat="server" CssClass="form-control"
                    placeholder="Ej: Ingeniería en Sistemas"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="ddlSemestre">Semestre</label>
                <asp:DropDownList ID="ddlSemestre" runat="server" CssClass="form-control">
                    <asp:ListItem Value="" Text="-- Selecciona --" />
                    <asp:ListItem Value="1" Text="1° Semestre" />
                    <asp:ListItem Value="2" Text="2° Semestre" />
                    <asp:ListItem Value="3" Text="3° Semestre" />
                    <asp:ListItem Value="4" Text="4° Semestre" />
                    <asp:ListItem Value="5" Text="5° Semestre" />
                    <asp:ListItem Value="6" Text="6° Semestre" />
                    <asp:ListItem Value="7" Text="7° Semestre" />
                    <asp:ListItem Value="8" Text="8° Semestre" />
                    <asp:ListItem Value="9" Text="9° Semestre" />
                    <asp:ListItem Value="10" Text="10° Semestre" />
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label for="txtComentarios">Comentarios</label>
                <asp:TextBox ID="txtComentarios" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"
                    placeholder="Observaciones adicionales..."></asp:TextBox>
            </div>

            <div style="margin-top: 2rem;">
                <asp:Button ID="btnGuardar" runat="server" Text="💾 Guardar Registro" CssClass="btn btn-primary"
                    OnClick="btnGuardar_Click" />
                <asp:Button ID="btnLimpiar" runat="server" Text="🔄 Limpiar" CssClass="btn btn-secondary"
                    OnClick="btnLimpiar_Click" CausesValidation="false"
                    style="margin-left: 1rem; background: #64748b; color: white;" />
            </div>
        </div>

    </asp:Content>