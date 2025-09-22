
# 📌 Prompt Mestre Ajustado — API ERP Modular (.NET 8 / ASP.NET Core)

Este prompt foi ajustado para alinhar-se ao perfil do programador, priorizando **simplicidade (KISS/YAGNI)**,
evitando overengineering e mantendo foco em **ERP corporativo real**.

---

## 🎯 Objetivo
Gerar um projeto **API RESTful ASP.NET Core (.NET 8, C#)** para um **ERP modular (10+ módulos)**.

---

## ✅ Filosofia
Aplicar arquitetura limpa e sustentável, seguindo **KISS/YAGNI**, sem overengineering.  
Respeitar separação clara de responsabilidades:
- **Controllers** → entrada/saída  
- **Services** → regras de negócio  
- **Repositories** → acesso a dados  
- **DTOs/ViewModels** → transporte de dados  
- **Infrastructure** → persistência, integrações  
- **Core** → entidades, contratos, regras centrais  

---

## 📂 Estrutura do Projeto
Organizar em **monorepo** com as seguintes camadas e módulos:

- **Core** (entidades, contratos)  
- **Application** (serviços, DTOs, CQRS)  
- **Infrastructure** (EF Core, persistência)  
- **API** (Controllers/Endpoints)  
- **Modules** (SEG, RHU, FRE, TRE, MSO, REC, EST, COM, VND, DOC, REL)

---

## 🔒 Requisitos Obrigatórios
- Multiempresa/multifilial + campo `IdSaas` (multi-tenant).  
- Autenticação/autorização JWT.  
- Controle de permissões (usuário, grupo, menu, botão).  
- EF Core 8 + SQL Server 2019+.  
- CRUDs padrão com DTOs, validações e repositórios.  
- DataTables + AdminLTE no front.  
- Logging básico com Serilog.  
- Testes unitários simples (xUnit).  

---

## 📌 Instruções de Resposta
A IA deve entregar:

- Código **completo e pronto para compilar**.  
- Caminhos exatos de arquivos (ex: `src/Modules/SEG/Application/Services/UsuarioService.cs`).  
- CRUDs de exemplo em pelo menos 1 módulo.  
- `Program.cs`, `DbContext`, models, DTOs, controllers e services.  
- Explicações rápidas e objetivas (sem textos desnecessários).  
- ❌ Não adicionar funcionalidades não solicitadas.  

---

## 🚀 Resultado Esperado
Um **esqueleto funcional de ERP modular em .NET 8**, claro, direto e expansível.
