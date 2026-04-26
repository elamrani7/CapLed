import { BrowserRouter } from 'react-router-dom';
import { CartProvider } from './context/CartContext';
import { AuthProvider } from './context/AuthContext';
import { AppRouter } from './router/AppRouter';

function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <BrowserRouter>
          <AppRouter />
          {/* WhatsApp floating button — PartFinder style */}
          <a
            href="https://wa.me/212702380380"
            target="_blank"
            rel="noopener noreferrer"
            className="pf-whatsapp"
            title="Contactez-nous sur WhatsApp">
            <i className="bi bi-whatsapp"></i>
          </a>
        </BrowserRouter>
      </CartProvider>
    </AuthProvider>
  );
}

export default App;
