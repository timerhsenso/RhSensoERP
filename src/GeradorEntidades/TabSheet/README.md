# 🗂️ TabSheet Generator - FASE 3 COMPLETA

Gerador de telas Mestre/Detalhe com abas (TabSheets) para o RhSensoERP.

## ✅ Versão 1.1.0 (FASE 3 - Interface Completa)

**O que inclui:**
- ✅ FASE 1: Modelos e Atributos
- ✅ FASE 2: Service e Templates de geração
- ✅ FASE 3: Controller, Modal UI e JavaScript

## 📁 Estrutura

```
TabSheet/
├── Attributes/
│   └── RelationshipAttributes.cs      # [MasterEntity], [DetailEntity], etc.
├── Controllers/                        # NOVO - FASE 3
│   └── TabSheetController.cs           # API endpoints
├── Models/
│   └── TabSheetModels.cs               # Configurações
├── Services/
│   └── TabSheetGeneratorService.cs     # Geração
├── Templates/
│   ├── TabSheetEntityTemplate.cs       # Gera entidades
│   ├── TabSheetViewTemplate.cs         # Gera Views
│   └── TabSheetJavaScriptTemplate.cs   # Gera JavaScript
├── Views/                              # NOVO - FASE 3
│   └── _TabSheetModal.cshtml           # Modal de configuração
├── wwwroot/                            # NOVO - FASE 3
│   └── js/
│       └── tabsheet-config.js          # JavaScript do modal
├── Examples/
│   └── EntidadesExemplo.cs
├── README.md
└── INTEGRACAO.md                       # Instruções de integração
```

## 🚀 Instalação Rápida

### 1. Copiar pasta para o projeto
```
GeradorFullStack/
└── TabSheet/   ← COPIAR AQUI
```

### 2. Registrar no DI
```csharp
// Program.cs
builder.Services.AddScoped<TabSheetGeneratorService>();
```

### 3. Adicionar botão na UI
```html
<button type="button" class="btn btn-info" id="btnGerarTabSheet">
    <i class="fas fa-layer-group mr-1"></i>TabSheet
</button>
```

### 4. Incluir o modal
```html
@await Html.PartialAsync("~/TabSheet/Views/_TabSheetModal.cshtml")
```

### 5. Incluir o JavaScript
```html
<script src="~/js/tabsheet-config.js"></script>
```

**Veja `INTEGRACAO.md` para instruções detalhadas.**

## 🎯 Como Usar

1. Selecionar tabela na lista
2. Clicar "TabSheet"
3. Modal abre com:
   - Dados do mestre pré-preenchidos
   - Lista de tabelas candidatas (com FK para o mestre)
4. Adicionar abas desejadas
5. Configurar cada aba (título, ícone, permissões)
6. Clicar "Gerar e Baixar ZIP"

## 📦 O que é gerado

| Arquivo | Descrição |
|---------|-----------|
| `{Entity}.cs` | Entidade mestre com `[MasterEntity]` |
| `{Detail}.cs` | Entidades detalhe com `[DetailEntity]` |
| `Edit.cshtml` | View principal com tabs (AdminLTE) |
| `_{Tab}Tab.cshtml` | Partial Views com DataTable + Modal |
| `{entity}-tabsheet.js` | JavaScript (CRUD AJAX) |
| `{id}.tabsheet.json` | Configuração JSON |
| `README.md` | Documentação do gerado |

## 🔌 API Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/tabsheet/config/{tableName}` | Obtém configuração inicial |
| POST | `/api/tabsheet/validate` | Valida configuração |
| POST | `/api/tabsheet/generate` | Gera arquivos (JSON) |
| POST | `/api/tabsheet/generate/zip` | Gera e baixa ZIP |
| POST | `/api/tabsheet/preview/{fileType}` | Preview de arquivo |

## 🔧 Dependências

- `DatabaseService` existente
- AdminLTE 3.x
- DataTables
- jQuery
- Bootstrap 4/5
- Toastr
- SweetAlert2 (opcional)

## 📋 Changelog

### v1.1.0 (FASE 3)
- ✅ TabSheetController com endpoints
- ✅ Modal de configuração completo
- ✅ JavaScript para interação
- ✅ Preview de arquivos
- ✅ Download ZIP
- ✅ Documentação de integração

### v1.0.1 (FASE 2)
- ✅ TabSheetGeneratorService
- ✅ Templates de geração
- ✅ Correção de propriedades do ColunaInfo

### v1.0.0 (FASE 1)
- ✅ Modelos de configuração
- ✅ Atributos de relacionamento
- ✅ Exemplo de uso
