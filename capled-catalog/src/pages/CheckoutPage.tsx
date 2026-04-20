import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';
import { leadApi } from '../api/leadApi';
import { getErrorMessage } from '../utils/apiErrors';

export const CheckoutPage = () => {
  const { cartItems, totalItems, clearCart } = useCart();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    nomProprietaire: '',
    societe: '',
    email: '',
    telephone: ''
  });
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  if (cartItems.length === 0 && !loading) {
    return (
      <div className="d-flex flex-column min-vh-100 bg-light">
        <Navbar />
        <main className="flex-grow-1 d-flex flex-column align-items-center justify-content-center text-center">
          <p className="fs-5 text-secondary mb-3">Votre panier est vide.</p>
          <button onClick={() => navigate('/catalogue')} className="btn btn-link text-primary">
            Retour au catalogue
          </button>
        </main>
        <Footer />
      </div>
    );
  }

  const totalQuantity = cartItems.reduce((acc: number, item: any) => acc + item.quantity, 0);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const payload = {
        nomClient: formData.nomProprietaire,
        societe: formData.societe,
        emailClient: formData.email,
        telephone: formData.telephone,
        sourceAcquisition: "SITE_WEB",
        lignes: cartItems.map((item: any) => ({
          articleId: item.articleId,
          quantiteDemandee: item.quantity
        }))
      };

      await leadApi.postLead(payload);
      
      clearCart();
      navigate('/success');

    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="d-flex flex-column min-vh-100 bg-light">
      <Navbar />
      
      <main className="flex-grow-1 container py-5">
        <h1 className="fw-bolder text-dark mb-5 border-bottom pb-3">Validation de votre Devis</h1>

        <div className="row g-5">
          
          {/* Colonne Formulaire */}
          <div className="col-lg-8">
            <div className="alert alert-warning mb-4 fw-bold">
              <i className="bi bi-info-circle-fill me-2"></i>
              Ceci est une demande de devis sans obligation d'achat. Un conseiller vous contactera avec les tarifs et délais.
            </div>
            
            <div className="card shadow-sm border-0 rounded-3">
              <div className="card-header bg-white border-bottom-0 pt-4 px-4 pb-0">
                <h4 className="fw-bolder mb-0 text-dark">Coordonnées de Facturation / Contact</h4>
              </div>
              <div className="card-body p-4">
                {error && (
                  <div className="alert alert-danger" role="alert">
                    <i className="bi bi-exclamation-circle me-2"></i> {error}
                  </div>
                )}

                <form onSubmit={handleSubmit} className="row g-4 bg-light p-3 rounded">
                  <div className="col-md-6">
                    <label className="form-label fw-bold text-dark">
                      Nom complet <span className="text-danger">*</span>
                    </label>
                    <input 
                      type="text" 
                      name="nomProprietaire" 
                      required 
                      value={formData.nomProprietaire}
                      onChange={handleChange}
                      className="form-control"
                      placeholder="Jean Dupont"
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label fw-bold text-dark">
                      Société / Entreprise
                    </label>
                    <input 
                      type="text" 
                      name="societe" 
                      value={formData.societe}
                      onChange={handleChange}
                      className="form-control"
                      placeholder="Nom de l'entreprise"
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label fw-bold text-dark">
                      Email <span className="text-danger">*</span>
                    </label>
                    <input 
                      type="email" 
                      name="email" 
                      required 
                      value={formData.email}
                      onChange={handleChange}
                      className="form-control"
                      placeholder="jean.dupont@societe.com"
                    />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label fw-bold text-dark">
                      Téléphone <span className="text-danger">*</span>
                    </label>
                    <input 
                      type="tel" 
                      name="telephone" 
                      required 
                      value={formData.telephone}
                      onChange={handleChange}
                      className="form-control"
                      placeholder="01 23 45 67 89"
                    />
                  </div>
                  
                  <div className="col-12 mt-4 pt-4 border-top">
                    <button 
                      type="submit" 
                      disabled={loading}
                      className="btn btn-warning btn-lg w-100 fw-bolder shadow-sm d-flex align-items-center justify-content-center gap-2 border-dark border-opacity-10 py-3 text-dark text-uppercase fs-5"
                    >
                      {loading ? (
                        <>
                          <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                          Envoi en cours...
                        </>
                      ) : (
                        <>
                          <i className="bi bi-send-fill"></i> Envoyer la demande de devis
                        </>
                      )}
                    </button>
                    <p className="small text-center text-muted mt-3 mb-0">
                      Vos données seront traitées par notre équipe commerciale pour vous répondre dans les plus brefs délais.
                    </p>
                  </div>
                </form>
              </div>
            </div>
          </div>

          {/* Colonne Récapitulatif */}
          <div className="col-lg-4">
            <div className="card bg-white border border-secondary border-opacity-25 shadow-sm sticky-top" style={{ top: '6rem' }}>
              <div className="card-header bg-transparent border-bottom-0 pt-4 px-4 pb-0">
                <h5 className="fw-bold m-0">Résumé de la demande</h5>
              </div>
              <div className="card-body px-4">
                
                <div className="d-flex justify-content-between align-items-center mb-3 text-secondary">
                  <span>Articles</span>
                  <span className="fw-bold text-dark">{totalQuantity}</span>
                </div>
                
                <div className="d-flex align-items-center gap-2 mb-4 p-2 bg-light rounded">
                  <i className="bi bi-tag-fill text-primary"></i>
                  <span className="small text-muted">Prix communiqué par devis</span>
                </div>
                <hr className="border-secondary opacity-25" />
                
                <div className="mt-3">
                  <ul className="list-unstyled mb-0 d-flex flex-column gap-2 small">
                    {cartItems.map((item: any) => (
                      <li key={item.articleId} className="d-flex justify-content-between gap-3">
                        <span className="text-muted text-truncate" title={item.nom} style={{ maxWidth: '75%' }}>
                          <span className="fw-bold text-dark me-1">{item.quantity}x</span> {item.nom}
                        </span>
                        <span className="text-dark fw-medium text-end">
                          x{item.quantity}
                        </span>
                      </li>
                    ))}
                  </ul>
                </div>
              </div>
            </div>
          </div>

        </div>
      </main>

      <Footer />
    </div>
  );
};
