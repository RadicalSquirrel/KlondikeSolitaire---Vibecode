# Klondike Solitaire - Blazor WebAssembly

A modern, interactive web-based implementation of Klondike Solitaire built with Blazor WebAssembly.

## Features

- **Interactive Gameplay**: Click-based card movement with visual feedback
- **Customizable Rules**:
  - Draw 1 or 3 cards from stock
  - King-only or any card in empty tableau
  - Optional Foundation→Tableau moves
  - Configurable stock redeal limits
- **Scoring System**: Track points and time
- **Statistics Tracking**: Persistent game statistics stored in browser localStorage
- **Responsive Design**: Beautiful card animations and green felt table aesthetic
- **Undo Functionality**: Unlimited undo of moves
- **Win Detection**: Automatic celebration when you win!

## Running the Application

### Development Mode

```bash
cd KlondikeSolitaire.Blazor
dotnet run
```

Then open your browser to `https://localhost:5001` (or the URL shown in the terminal).

### Build for Production

```bash
dotnet publish -c Release
```

The published files will be in `bin/Release/net8.0/publish/wwwroot/` and can be hosted on any static web server.

## Project Structure

```
KlondikeSolitaire.Blazor/
├── Components/
│   ├── CardComponent.razor       # Individual card display
│   ├── GameBoard.razor           # Main game board with tableau, foundations, etc.
│   └── GameSettings.razor        # New game configuration dialog
├── Pages/
│   ├── Home.razor                # Main game page
│   └── Statistics.razor          # Statistics display page
├── Services/
│   ├── GameService.cs            # Game state management
│   └── StatisticsService.cs     # Statistics persistence (localStorage)
└── wwwroot/
    └── css/
        └── game.css              # Game-specific styles
```

## How to Play

1. **Start a New Game**: Click "New Game" and configure your preferred rules
2. **Draw Cards**: Click the stock pile to draw cards
3. **Move Cards**: Click a card to select it, then click the destination
4. **Build Foundations**: Move Aces to foundations and build up by suit
5. **Build Tableau**: Alternate red/black cards in descending order
6. **Undo Moves**: Click "Undo" to reverse your last move
7. **Win**: Get all 52 cards into the four foundation piles!

## Technologies Used

- **Blazor WebAssembly** (.NET 8)
- **KlondikeSolitaire.Core** - Shared game logic library
- **Browser LocalStorage** - For statistics persistence
- **CSS3** - Animations and styling

## Browser Compatibility

Works in all modern browsers that support WebAssembly:
- Chrome/Edge 91+
- Firefox 89+
- Safari 15+
