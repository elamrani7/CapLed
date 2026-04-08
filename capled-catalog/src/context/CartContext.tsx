import { createContext, useContext, useState, useEffect } from 'react';

const CartContext = createContext<any>(null);

export const useCart = () => useContext(CartContext);

export const CartProvider = ({ children }: { children: React.ReactNode }) => {
  const [cartItems, setCartItems] = useState<any[]>(() => {
    const saved = localStorage.getItem('capled_cart');
    return saved ? JSON.parse(saved) : [];
  });

  useEffect(() => {
    localStorage.setItem('capled_cart', JSON.stringify(cartItems));
  }, [cartItems]);

  const addToCart = (product: any, quantity = 1) => {
    setCartItems((prev: any[]) => {
      const existing = prev.find(item => item.articleId === product.id);
      if (existing) {
        return prev.map(item => 
          item.articleId === product.id 
            ? { ...item, quantity: item.quantity + quantity }
            : item
        );
      }
      return [...prev, { 
        articleId: product.id, 
        nom: product.nom, 
        prixVente: product.prixVente, 
        quantity 
      }];
    });
  };

  const removeFromCart = (articleId: number) => {
    setCartItems((prev: any[]) => prev.filter(item => item.articleId !== articleId));
  };

  const updateQuantity = (articleId: number, newQty: number) => {
    if (newQty < 1) return;
    setCartItems((prev: any[]) => prev.map(item => 
      item.articleId === articleId 
        ? { ...item, quantity: newQty }
        : item
    ));
  };

  const clearCart = () => setCartItems([]);

  const totalItems = cartItems.reduce((acc: number, item: any) => acc + item.quantity, 0);

  return (
    <CartContext.Provider value={{ cartItems, addToCart, removeFromCart, updateQuantity, clearCart, totalItems }}>
      {children}
    </CartContext.Provider>
  );
};
