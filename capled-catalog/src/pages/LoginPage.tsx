import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';
import httpClient from '../api/httpClient';
import { getErrorMessage } from '../utils/apiErrors';

export const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const data = await httpClient.post('/api/v1/ClientAuth/login', { email, password });

      login({
        token: data.token,
        email: data.email,
        fullName: data.fullName,
        clientId: data.clientId,
        expiresAt: data.expiresAt,
      });

      navigate('/catalogue');
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <Navbar />
      <div className="container py-5">
        <div className="row justify-content-center">
          <div className="col-md-5 col-lg-4">
            <div className="card shadow-sm border-0" style={{ borderTop: '4px solid #0056A6' }}>
              <div className="card-body p-4 p-md-5">
                <div className="text-center mb-4">
                  <i className="bi bi-shield-lock" style={{ fontSize: '2.5rem', color: '#0056A6' }}></i>
                  <h3 className="fw-bold mt-2 mb-1">Connexion</h3>
                  <p className="text-muted small">Accédez à votre espace client</p>
                </div>

                {error && (
                  <div className="alert alert-danger py-2 d-flex align-items-center gap-2" role="alert">
                    <i className="bi bi-exclamation-triangle-fill"></i>
                    <span>{error}</span>
                  </div>
                )}

                <form onSubmit={handleSubmit}>
                  <div className="mb-3">
                    <label className="form-label small fw-semibold">Adresse e-mail</label>
                    <div className="input-group">
                      <span className="input-group-text bg-light"><i className="bi bi-envelope"></i></span>
                      <input 
                        type="email" 
                        className="form-control" 
                        value={email} 
                        onChange={(e) => setEmail(e.target.value)} 
                        required 
                        placeholder="vous@exemple.com"
                        autoComplete="email"
                      />
                    </div>
                  </div>

                  <div className="mb-3">
                    <label className="form-label small fw-semibold">Mot de passe</label>
                    <div className="input-group">
                      <span className="input-group-text bg-light"><i className="bi bi-lock"></i></span>
                      <input 
                        type="password" 
                        className="form-control" 
                        value={password} 
                        onChange={(e) => setPassword(e.target.value)} 
                        required 
                        placeholder="Votre mot de passe"
                        autoComplete="current-password"
                      />
                    </div>
                  </div>

                  <button type="submit" className="btn btn-primary w-100 py-2 fw-bold" disabled={loading}>
                    {loading ? (
                      <><span className="spinner-border spinner-border-sm me-2"></span>Connexion...</>
                    ) : (
                      <><i className="bi bi-box-arrow-in-right me-2"></i>Se connecter</>
                    )}
                  </button>
                </form>

                <hr className="my-4" />

                <p className="text-center text-muted mb-0 small">
                  Pas encore de compte ? <Link to="/register" className="text-primary fw-semibold">Créer un compte</Link>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
      <Footer />
    </>
  );
};
