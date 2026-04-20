import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';
import { QuantitySelector } from '../components/shared/QuantitySelector';

export const CartPage = () => {
  const { cartItems, removeFromCart, updateQuantity } = useCart();
  const navigate = useNavigate();

  return (
    <div className="d-flex flex-column min-vh-100 bg-light">
      <Navbar />
      
      <main className="flex-grow-1 container py-5">
        <h1 className="fw-bolder text-dark mb-5 pb-3 border-bottom">Demande de Devis</h1>

        {cartItems.length === 0 ? (
          <div className="card shadow-sm border-0 text-center py-5 mt-5">
            <div className="card-body py-5">
              <div className="display-1 text-muted opacity-50 mb-4"><i className="bi bi-cart-x"></i></div>
              <h2 className="fw-bold mb-3">Votre liste est vide</h2>
              <p className="text-secondary mb-4">Ajoutez des équipements depuis le catalogue pour constituer votre demande de devis.</p>
              <Link to="/catalogue" className="btn btn-primary btn-lg px-4 shadow-sm">
                Parcourir le catalogue
              </Link>
            </div>
          </div>
        ) : (
          <div className="row g-4">
            
            {/* Colonne Liste des articles */}
            <div className="col-lg-8">
              <div className="table-responsive bg-white rounded shadow-sm border-0">
                <table className="table table-hover align-middle mb-0 eav-table">
                  <thead className="bg-light">
                    <tr>
                      <th scope="col" className="ps-4">Article</th>
                      <th scope="col" className="text-center">Quantité</th>
                      <th scope="col" className="text-center pe-4">Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    {cartItems.map((item: any) => (
                      <tr key={item.articleId}>
                        <td className="ps-4 py-3">
                          <div className="d-flex align-items-center gap-3">
                            <div className="bg-light border rounded overflow-hidden flex-shrink-0 d-flex justify-content-center align-items-center" style={{ width: '80px', height: '80px' }}>
                              {item.image ? (
                                <img src={item.image} alt={item.nom} className="w-100 h-100 object-fit-contain p-1" />
                              ) : (
                                <i className="bi bi-image text-muted fs-4"></i>
                              )}
                            </div>
                            <div>
                              <h6 className="mb-1 fw-bold text-dark lh-sm" style={{ maxWidth: '300px' }}>
                                <Link to={`/catalogue/${item.articleId}`} className="text-dark text-decoration-none">
                                  {item.nom}
                                </Link>
                              </h6>
                              {item.reference && <small className="text-muted font-monospace">Réf: {item.reference}</small>}
                            </div>
                          </div>
                        </td>
                        <td className="text-center">
                          <QuantitySelector 
                            quantity={item.quantity} 
                            onChange={(val) => updateQuantity(item.articleId, val)} 
                          />
                        </td>
                        <td className="text-center pe-4">
                          <button 
                            onClick={() => removeFromCart(item.articleId)}
                            className="btn btn-outline-secondary btn-sm border-0"
                            title="Retirer"
                          >
                            <i className="bi bi-trash fs-5 text-danger"></i>
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Colonne Résumé */}
            <div className="col-lg-4">
              <div className="card shadow-sm border-0 bg-white" style={{ position: 'sticky', top: '120px' }}>
                <div className="card-body p-4">
                  <h5 className="fw-bold mb-4 pb-3 border-bottom text-uppercase fs-6">Récapitulatif de la demande</h5>
                  
                  <div className="d-flex justify-content-between align-items-center mb-4">
                    <span className="text-secondary fw-bold">Articles demandés</span>
                    <span className="fs-3 fw-bolder text-primary lh-1">{cartItems.reduce((acc: number, item: any) => acc + item.quantity, 0)}</span>
                  </div>

                  <div className="d-flex align-items-center gap-2 mb-3 p-2 bg-light rounded">
                    <i className="bi bi-tag-fill text-primary"></i>
                    <span className="small text-muted">Le prix sera communiqué dans le devis personnalisé par notre équipe commerciale.</span>
                  </div>

                  <div className="alert alert-warning d-flex gap-2" role="alert">
                    <i className="bi bi-exclamation-triangle-fill flex-shrink-0"></i>
                    <span className="small">
                      Ceci n'est pas une commande. En validant, vous transmettrez une demande de devis à notre équipe.
                    </span>
                  </div>

                  <button 
                    onClick={() => navigate('/checkout')}
                    className="btn btn-success btn-lg w-100 fw-bold shadow-sm d-flex justify-content-center align-items-center gap-2 mt-4"
                  >
                    Valider mon devis <i className="bi bi-chevron-right"></i>
                  </button>
                </div>
              </div>
            </div>

          </div>
        )}
      </main>

      <Footer />
    </div>
  );
};
