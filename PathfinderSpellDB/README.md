# Pathfinder 2E Spell Database - .NET Edition

Uma adaptação em .NET Core Blazor do aplicativo original Pathfinder 2E Spell Database, desenvolvido em JavaScript/React.

## 🎯 Funcionalidades

- **Base de Dados Completa**: Busca através de centenas de magias do Pathfinder 2E
- **Filtros Avançados**: Filtre por tradições, tipos, níveis, traços e muito mais
- **Sistema de Favoritos**: Salve suas magias favoritas em listas personalizadas
- **Preparação Vanciana**: Acompanhe a preparação de magias para conjuradores Vancianos
- **Tema Escuro/Claro**: Escolha seu modo de visualização preferido
- **Design Responsivo**: Funciona perfeitamente em desktop e dispositivos móveis
- **Capacidade Offline**: Todos os dados são carregados localmente para acesso rápido

## 🛠️ Tecnologias Utilizadas

- **Backend**: .NET 8 com Blazor Server
- **Frontend**: Blazor Components com Bootstrap 5
- **Ícones**: Font Awesome 6
- **Dados**: Arquivos JSON com informações das magias

## 🚀 Como Executar

### Pré-requisitos

- .NET 8 SDK
- Navegador web moderno

### Instalação

1. Clone o repositório:
```bash
git clone <url-do-repositorio>
cd PathfinderSpellDB
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Execute o projeto:
```bash
dotnet run
```

4. Abra seu navegador e acesse: `http://localhost:5128`

## 📁 Estrutura do Projeto

```
PathfinderSpellDB/
├── Components/           # Componentes Blazor
│   ├── Layout/          # Layouts e navegação
│   ├── Pages/           # Páginas da aplicação
│   └── *.razor          # Componentes reutilizáveis
├── Data/                # Dados JSON das magias
├── Models/              # Modelos C# (Spell, SpellType, etc.)
├── Services/            # Serviços de dados
└── wwwroot/             # Arquivos estáticos
```

## 🎮 Como Usar

### Busca de Magias
1. Use a barra de pesquisa para encontrar magias por nome
2. Selecione filtros como tradição, tipo ou nível
3. Escolha entre visualização em lista ou detalhes
4. Ordene por nome, nível ou ações

### Sistema de Favoritos
1. Navegue até a página "Bookmarks"
2. Crie uma nova lista de favoritos
3. Adicione magias às suas listas
4. Para listas Vancianas, acompanhe a preparação de magias

### Temas
- Use o botão no canto superior direito para alternar entre tema claro e escuro
- Sua preferência é salva no navegador

## 📊 Dados

Todos os dados de magias são provenientes do [Archives of Nethys](https://2e.aonprd.com/), o site oficial de referência do Pathfinder 2E. Este aplicativo não é afiliado à Paizo Inc.

## ⚖️ Aviso Legal

**Política de Uso Comunitário**: Este site e aplicativo usa marcas registradas e/ou direitos autorais de propriedade da Paizo Inc., que são usados sob a Política de Uso Comunitário da Paizo. É expressamente proibido cobrar pelo uso ou acesso a este conteúdo. Este site e aplicativo não é publicado, endossado ou especificamente aprovado pela Paizo Inc. Para mais informações sobre a Política de Uso Comunitário da Paizo, visite [paizo.com/communityuse](https://paizo.com/communityuse).

## 🔧 Desenvolvimento

### Adicionando Novas Funcionalidades

1. **Modelos**: Adicione novos modelos em `Models/`
2. **Serviços**: Implemente novos serviços em `Services/`
3. **Componentes**: Crie componentes reutilizáveis em `Components/`
4. **Páginas**: Adicione novas páginas em `Components/Pages/`

### Estrutura de Dados

Os dados das magias estão em formato JSON em `Data/spells.json` e `Data/spellTypes.json`. Cada magia contém:

- Nome, tipo e nível
- Tradições (Arcana, Divina, Oculta, Primal)
- Ações necessárias
- Alcance, área e duração
- Descrição e efeitos ampliados
- Fonte e link para Archives of Nethys

## 📈 Estatísticas

- **Total de Magias**: Mais de 1000 magias
- **Cantrips**: Magias de nível 0
- **Poderes de Foco**: Magias especiais de foco
- **Magias Regulares**: Magias de nível 1-10

## 🤝 Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para:

1. Reportar bugs
2. Sugerir novas funcionalidades
3. Enviar pull requests
4. Melhorar a documentação

## 📄 Licença

Este projeto está licenciado sob a Licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 🙏 Agradecimentos

- [Paizo Inc.](https://paizo.com/) - Criadores do Pathfinder
- [Archives of Nethys](https://2e.aonprd.com/) - Fonte dos dados
- Comunidade Pathfinder 2E
- Desenvolvedores do projeto original JavaScript/React

---

**Nota**: Este é um projeto de adaptação educacional. Todos os direitos das regras e conteúdo do Pathfinder 2E pertencem à Paizo Inc.