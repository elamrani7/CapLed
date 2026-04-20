import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';

const API_BASE = 'http://localhost:5000';

export const EmailConfirmationPage = () => {
  const [searchParams] = useSearchParams();
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [message, setMessage] = useState('');

  useEffect(() => {
    const token = searchParams.get('token');
    const email = searchParams.get('email');

    if (!token || !email) {
      setStatus('error');
      setMessage('Lien de confirmation invalide. Paramètres manquants.');
      return;
    }

    const confirmEmail = async () => {
      try {
        const res = await fetch(
          `${API_BASE}/api/v1/ClientAuth/confirm-email?token=${encodeURIComponent(token)}&email=${encodeURIComponent(email)}`
        );
        const data = await res.json();

        if (res.ok) {
          setStatus('success');
          setMessage(data.message || 'Votre compte a été confirmé avec succès !');
        } else {
          setStatus('error');
          setMessage(data.error || 'La confirmation a échoué.');
        }
      } catch {
        setStatus('error');
        setMessage('Impossible de contacter le serveur. Réessayez plus tard.');
      }
    };

    confirmEmail();
  }, [searchParams]);

  return (
    <>
      <Navbar />
      <div className="container py-5">
        <div className="row justify-content-center">
          <div className="col-md-6 col-lg-5">
            <div className="card shadow-sm border-0" style={{ 
              borderTop: `4px solid ${status === 'success' ? '#2ECC71' : status === 'error' ? '#D92B2B' : '#0056A6'}` 
            }}>
              <div className="card-body text-center p-5">
                {status === 'loading' && (
                  <>
                    <div className="spinner-border text-primary mb-4" style={{ width: '3rem', height: '3rem' }} role="status">
                      <span className="visually-hidden">Chargement...</span>
                    </div>
                    <h4 className="fw-bold mb-2">Confirmation en cours...</h4>
                    <p className="text-muted">Veuillez patienter pendant que nous vérifions votre lien.</p>
                  </>
                )}

                {status === 'success' && (
                  <>
                    <div className="mb-4">
                      <i className="bi bi-check-circle-fill" style={{ fontSize: '4rem', color: '#2ECC71' }}></i>
                    </div>
                    <h3 className="fw-bold mb-3">Compte confirmé !</h3>
                    <p className="text-muted mb-4">{message}</p>
                    <Link to="/login" className="btn btn-primary px-4 py-2 fw-bold">
                      <i className="bi bi-box-arrow-in-right me-2"></i>Se connecter
                    </Link>
                  </>
                )}

                {status === 'error' && (
                  <>
                    <div className="mb-4">
                      <i className="bi bi-x-circle-fill" style={{ fontSize: '4rem', color: '#D92B2B' }}></i>
                    </div>
                    <h3 className="fw-bold mb-3">Échec de la confirmation</h3>
                    <p className="text-muted mb-4">{message}</p>
                    <div className="d-flex gap-2 justify-content-center flex-wrap">
                      <Link to="/register" className="btn btn-outline-primary">
                        <i className="bi bi-person-plus me-2"></i>Créer un compte
                      </Link>
                      <Link to="/login" className="btn btn-outline-secondary">
                        <i className="bi bi-box-arrow-in-right me-2"></i>Se connecter
                      </Link>
                    </div>
                  </>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
};
