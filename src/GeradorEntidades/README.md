# 🚀 INSTRUÇÕES PASSO A PASSO

## Como integrar o Wizard no seu projeto ASP.NET Core

---

## 📦 PASSO 1: Copiar os arquivos

### 1.1 Copiar a pasta `Models`

```
De:  wizard-integrado/Models/WizardRequest.cs
Para: GeradorEntidades/Models/WizardRequest.cs
```

**Como fazer:**
1. Abra o Windows Explorer
2. Vá até a pasta onde você extraiu o ZIP
3. Copie o arquivo `Models/WizardRequest.cs`
4. Cole na pasta `Models` do seu projeto `GeradorEntidades`

---

### 1.2 Copiar os Controllers

```
De:  wizard-integrado/Controllers/GeneratorController.cs
Para: GeradorEntidades/Controllers/GeneratorController.cs

De:  wizard-integrado/Controllers/Api/GeneratorApiController.cs
Para: GeradorEntidades/Controllers/Api/GeneratorApiController.cs
```

**Como fazer:**
1. Copie `Controllers/GeneratorController.cs` para a pasta `Controllers` do seu projeto
2. Crie uma pasta `Api` dentro de `Controllers` (se não existir)
3. Copie `Controllers/Api/GeneratorApiController.cs` para essa pasta

---

### 1.3 Copiar a View

```
De:  wizard-integrado/Views/Generator/Index.cshtml
Para: GeradorEntidades/Views/Generator/Index.cshtml
```

**Como fazer:**
1. Crie uma pasta `Generator` dentro da pasta `Views` do seu projeto
2. Copie `Views/Generator/Index.cshtml` para essa nova pasta

---

### 1.4 Copiar os arquivos wwwroot

```
De:  wizard-integrado/wwwroot/css/generator/
Para: GeradorEntidades/wwwroot/css/generator/

De:  wizard-integrado/wwwroot/js/generator/
Para: GeradorEntidades/wwwroot/js/generator/
```

**Como fazer:**
1. Crie a pasta `generator` dentro de `wwwroot/css`
2. Copie `wwwroot/css/generator/wizard.css` para ela
3. Crie a pasta `generator` dentro de `wwwroot/js`
4. Copie toda a pasta `wwwroot/js/generator/` (app.js + modules/)

---

## ⚙️ PASSO 2: Registrar serviços no Program.cs

Abra o arquivo `Program.cs` e adicione (se ainda não tiver):

```csharp
// Adicionar no início, junto com os outros services
builder.Services.AddScoped<FullStackGeneratorService>();
builder.Services.AddScoped<ManifestService>();
```

---

## 🌐 PASSO 3: Testar

1. Execute o projeto (F5 no Visual Studio)
2. Abra o navegador
3. Acesse: `https://localhost:PORTA/Generator`
4. O Wizard deve aparecer!

---

## 🗂️ ESTRUTURA FINAL

Após copiar, seu projeto deve ficar assim:

```
GeradorEntidades/
├── Controllers/
│   ├── Api/
│   │   └── GeneratorApiController.cs    ←── NOVO
│   ├── GeneratorController.cs           ←── NOVO
│   ├── HomeController.cs
│   └── ManifestController.cs
├── Models/
│   ├── Models.cs
│   └── WizardRequest.cs                 ←── NOVO
├── Services/
│   ├── FullStackGeneratorService.cs
│   └── ManifestService.cs
├── Templates/
│   └── (todos os templates existentes)
├── Views/
│   ├── Generator/                       ←── NOVA PASTA
│   │   └── Index.cshtml                 ←── NOVO
│   └── Manifest/
│       └── Index.cshtml
└── wwwroot/
    ├── css/
    │   └── generator/                   ←── NOVA PASTA
    │       └── wizard.css               ←── NOVO
    └── js/
        └── generator/                   ←── NOVA PASTA
            ├── app.js                   ←── NOVO
            └── modules/                 ←── NOVA PASTA
                ├── api-client.js        ←── NOVO
                ├── form-designer.js     ←── NOVO
                ├── grid-config.js       ←── NOVO
                ├── manifest-manager.js  ←── NOVO
                ├── schema-validator.js  ←── NOVO
                └── wizard.js            ←── NOVO
```

---

## ❓ Deu erro?

### Erro: "Namespace não encontrado"
- Verifique se o namespace do arquivo corresponde à pasta
- Ex: `namespace GeradorEntidades.Controllers.Api;`

### Erro: "Service não encontrado"
- Adicione os services no `Program.cs` conforme PASSO 2

### Erro: "View não encontrada"
- Verifique se a pasta `Views/Generator` existe
- Verifique se o arquivo se chama `Index.cshtml`

### Erro: "CSS/JS não carrega"
- Verifique se as pastas `wwwroot/css/generator` e `wwwroot/js/generator` existem
- Faça Ctrl+Shift+R para limpar cache do navegador

---

## 🎉 Pronto!

Agora você tem:
- ✅ UI bonita do Wizard
- ✅ Templates C# funcionando (não duplicados!)
- ✅ Tudo integrado no mesmo projeto
