# 🔌 Integração do TabSheet Generator

## 📋 Passo a Passo

### 1. Copiar Arquivos

```
GeradorFullStack/
├── TabSheet/                           ← COPIAR TUDO
│   ├── Attributes/
│   ├── Controllers/                    ← NOVO
│   │   └── TabSheetController.cs
│   ├── Models/
│   ├── Services/
│   ├── Templates/
│   ├── Views/                          ← NOVO
│   │   └── _TabSheetModal.cshtml
│   └── wwwroot/                        ← NOVO
│       └── js/
│           └── tabsheet-config.js
```

### 2. Registrar no DI (Program.cs)

```csharp
// Adicionar após os outros services
builder.Services.AddScoped<TabSheetGeneratorService>();
```

### 3. Adicionar Botão na UI

No arquivo `Views/Home/Index.cshtml` (ou onde você lista as tabelas), adicione o botão:

```html
<!-- Junto aos outros botões de ação -->
<button type="button" class="btn btn-info" id="btnGerarTabSheet" 
        title="Gerar TabSheet (Mestre/Detalhe)">
    <i class="fas fa-layer-group mr-1"></i>TabSheet
</button>
```

### 4. Incluir o Modal

No final do seu layout ou view principal, adicione:

```html
<!-- Modal TabSheet -->
@await Html.PartialAsync("~/TabSheet/Views/_TabSheetModal.cshtml")
```

### 5. Incluir o JavaScript

No seu `_Layout.cshtml` ou na seção Scripts:

```html
<script src="~/js/tabsheet-config.js"></script>
```

Ou mova o arquivo para `wwwroot/js/` e referencie:

```html
<script src="~/js/tabsheet-config.js"></script>
```

### 6. Dependências (já deve ter)

Certifique-se de que estas libs estão incluídas:
- jQuery
- Bootstrap 4/5
- Toastr (para notificações)
- AdminLTE (opcional, mas recomendado)

---

## 🎯 Como Usar

1. **Selecione uma tabela** na lista de tabelas
2. **Clique no botão "TabSheet"**
3. O modal abrirá com:
   - Informações da tabela mestre
   - Lista de tabelas relacionadas (que têm FK para o mestre)
4. **Adicione as abas** clicando no botão `+`
5. **Configure cada aba** (título, ícone, permissões)
6. **Clique em "Gerar e Baixar ZIP"**

---

## 📁 Arquivos Gerados

O ZIP conterá:

| Arquivo | Descrição |
|---------|-----------|
| `Entities/{Master}.cs` | Entidade mestre com `ICollection<>` |
| `Entities/{Detail}.cs` | Entidades de detalhe com FK |
| `Views/{Master}/Edit.cshtml` | View principal com tabs |
| `Views/{Master}/Partials/_{Tab}Tab.cshtml` | Partial views |
| `wwwroot/js/{master}-tabsheet.js` | JavaScript |
| `Config/{Id}.tabsheet.json` | Configuração JSON |
| `README.md` | Documentação |

---

## 🔧 Customização

### Alterar Módulos Disponíveis

No `TabSheetController.cs`, método `GetAvailableModules()`:

```csharp
private static List<ModuleOption> GetAvailableModules()
{
    return new List<ModuleOption>
    {
        new("GestaoDePessoas", "gestaodepessoas", "Gestão de Pessoas"),
        new("SeuModulo", "seumodulo", "Seu Módulo"),
        // Adicione mais...
    };
}
```

### Alterar Ícones Disponíveis

No mesmo controller, método `GetAvailableIcons()`.

### Personalizar Templates

Os templates estão em `TabSheet/Templates/`:
- `TabSheetEntityTemplate.cs` - Entidades
- `TabSheetViewTemplate.cs` - Views Razor
- `TabSheetJavaScriptTemplate.cs` - JavaScript

---

## ⚠️ Troubleshooting

### Erro: "Tabela não encontrada"
- Verifique se a conexão com o banco está configurada
- Verifique se a tabela existe e está acessível

### Erro: "Nenhuma tabela relacionada"
- A tabela selecionada precisa ter outras tabelas que referenciam ela via FK
- Verifique se as FKs estão definidas no banco

### Modal não abre
- Verifique se o JavaScript está carregado
- Verifique o console do navegador (F12)
- Certifique-se de que jQuery está carregado antes

### ZIP não baixa
- Verifique se o endpoint `/api/tabsheet/generate/zip` está funcionando
- Verifique o Network tab no DevTools

---

## 📞 Próximos Passos

Após a integração, você pode:

1. **Testar** com uma tabela que tenha relacionamentos
2. **Gerar** um TabSheet de exemplo
3. **Colocar** os arquivos gerados no projeto RhSensoERP
4. **Ajustar** conforme necessário

Boa sorte! 🚀
