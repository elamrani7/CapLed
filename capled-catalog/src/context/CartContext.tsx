import { createContext, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { useAuth } from './AuthContext';

const CartContext = createContext<any>(null);

export const useCart = () => useContext(CartContext);

const LEGACY_CART_KEY = 'capled_cart';
const GUEST_CART_KEY = 'partfinder_cart_guest';

const getCartStorageKey = (user: any) =>
  user?.clientId ? `partfinder_cart_client_${user.clientId}` : GUEST_CART_KEY;

const loadCart = (storageKey: string) => {
  try {
    const saved = localStorage.getItem(storageKey);
    if (saved) return JSON.parse(saved);

    if (storageKey === GUEST_CART_KEY) {
      const legacy = localStorage.getItem(LEGACY_CART_KEY);
      return legacy ? JSON.parse(legacy) : [];
    }
  } catch {
    return [];
  }

  return [];
};

const mergeCartItems = (targetItems: any[], sourceItems: any[]) => {
  const merged = [...targetItems];

  sourceItems.forEach((sourceItem) => {
    const existingIndex = merged.findIndex(item => item.articleId === sourceItem.articleId);
    if (existingIndex >= 0) {
      merged[existingIndex] = {
        ...merged[existingIndex],
        ...sourceItem,
        quantity: (merged[existingIndex].quantity || 0) + (sourceItem.quantity || 0),
      };
    } else {
      merged.push(sourceItem);
    }
  });

  return merged;
};

export const CartProvider = ({ children }: { children: React.ReactNode }) => {
  const { user } = useAuth();
  const storageKey = useMemo(() => getCartStorageKey(user), [user?.clientId]);
  const skipNextSave = useRef(false);
  const previousStorageKey = useRef(storageKey);
  const [cartItems, setCartItems] = useState<any[]>(() => loadCart(storageKey));

  useEffect(() => {
    const previousKey = previousStorageKey.current;
    previousStorageKey.current = storageKey;

    if (previousKey === GUEST_CART_KEY && storageKey !== GUEST_CART_KEY) {
      const mergedCart = mergeCartItems(loadCart(storageKey), loadCart(GUEST_CART_KEY));
      localStorage.setItem(storageKey, JSON.stringify(mergedCart));
      localStorage.removeItem(GUEST_CART_KEY);
      localStorage.removeItem(LEGACY_CART_KEY);

      skipNextSave.current = true;
      setCartItems(mergedCart);
      return;
    }

    skipNextSave.current = true;
    setCartItems(loadCart(storageKey));
  }, [storageKey]);

  useEffect(() => {
    if (skipNextSave.current) {
      skipNextSave.current = false;
      return;
    }

    localStorage.setItem(storageKey, JSON.stringify(cartItems));
  }, [cartItems, storageKey]);

  const addToCart = (product: any, quantity = 1) => {
    setCartItems((prev: any[]) => {
      const articleId = product.articleId ?? product.id;
      const existing = prev.find(item => item.articleId === articleId);
      if (existing) {
        return prev.map(item =>
          item.articleId === articleId
            ? {
                ...item,
                nom: product.nom ?? item.nom,
                prixVente: product.prixVente ?? item.prixVente,
                reference: product.reference ?? item.reference,
                image: product.image ?? item.image,
                images: product.images ?? item.images,
                urlImagePrincipale: product.urlImagePrincipale ?? item.urlImagePrincipale,
                quantity: item.quantity + quantity,
              }
            : item
        );
      }
      return [...prev, {
        articleId,
        nom: product.nom,
        prixVente: product.prixVente,
        reference: product.reference,
        image: product.image,
        images: product.images,
        urlImagePrincipale: product.urlImagePrincipale,
        quantity,
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
