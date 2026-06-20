import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';
import httpClient from '../api/httpClient';
import { getErrorMessage } from '../utils/apiErrors';

export const RegisterPage = () => {
  const [form, setForm] = useState({
    nom: '',
    prenom: '',
    email: '',
    password: '',
    confirmPassword: '',
    telephone: '',
    societe: '',
  });
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [error, setError] = useState('');

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (form.password !== form.confirmPassword) {
      setError('Les mots de passe ne correspondent pas.');
      return;
    }

    if (form.password.length < 6) {
      setError('Le mot de passe doit contenir au moins 6 caracteres.');
      return;
    }

    setLoading(true);
    try {
      const response = await httpClient.post('/api/v1/ClientAuth/register', {
        nom: form.nom,
        prenom: form.prenom || undefined,
        email: form.email,
        password: form.password,
        telephone: form.telephone || undefined,
        societe: form.societe || undefined,
      });

      setSuccessMessage(response?.message || 'Compte cree avec succes. Vous pouvez maintenant vous connecter.');
      setSuccess(true);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <>
        <Navbar />
        <div className="container py-5">
          <div className="row justify-content-center">
            <div className="col-md-6">
              <div className="card shadow-sm border-0" style={{ borderTop: '4px solid #2ECC71' }}>
                <div className="card-body text-center p-5">
                  <div className="mb-4">
                    <i className="bi bi-check-circle-fill" style={{ fontSize: '4rem', color: '#2ECC71' }}></i>
                  </div>
                  <h3 className="fw-bold mb-3">Compte cree avec succes</h3>
                  <p className="text-muted mb-4">
                    {successMessage}<br />
                    <strong>{form.email}</strong>
                  </p>
                  <Link to="/login" className="btn btn-outline-primary">
                    <i className="bi bi-box-arrow-in-right me-2"></i>Aller a la connexion
                  </Link>
                </div>
              </div>
            </div>
          </div>
        </div>
        <Footer />
      </>
    );
  }

  return (
    <>
      <Navbar />
      <div className="container py-5">
        <div className="row justify-content-center">
          <div className="col-md-6 col-lg-5">
            <div className="card shadow-sm border-0" style={{ borderTop: '4px solid #0056A6' }}>
              <div className="card-body p-4 p-md-5">
                <div className="text-center mb-4">
                  <i className="bi bi-person-plus" style={{ fontSize: '2.5rem', color: '#0056A6' }}></i>
                  <h3 className="fw-bold mt-2 mb-1">Creer un compte</h3>
                  <p className="text-muted small">Accedez a votre espace client PartFinder</p>
                </div>

                {error && (
                  <div className="alert alert-danger py-2 d-flex align-items-center gap-2" role="alert">
                    <i className="bi bi-exclamation-triangle-fill"></i>
                    <span>{error}</span>
                  </div>
                )}

                <form onSubmit={handleSubmit}>
                  <div className="row g-3">
                    <div className="col-6">
                      <label className="form-label small fw-semibold">Nom *</label>
                      <input type="text" name="nom" className="form-control" value={form.nom} onChange={handleChange} required />
                    </div>
                    <div className="col-6">
                      <label className="form-label small fw-semibold">Prenom</label>
                      <input type="text" name="prenom" className="form-control" value={form.prenom} onChange={handleChange} />
                    </div>
                  </div>

                  <div className="mt-3">
                    <label className="form-label small fw-semibold">Adresse e-mail *</label>
                    <div className="input-group">
                      <span className="input-group-text bg-light"><i className="bi bi-envelope"></i></span>
                      <input
                        type="email"
                        name="email"
                        className="form-control"
                        value={form.email}
                        onChange={handleChange}
                        required
                        placeholder="vous@exemple.com"
                      />
                    </div>
                  </div>

                  <div className="row g-3 mt-0">
                    <div className="col-6">
                      <label className="form-label small fw-semibold">Mot de passe *</label>
                      <div className="input-group">
                        <span className="input-group-text bg-light"><i className="bi bi-lock"></i></span>
                        <input
                          type="password"
                          name="password"
                          className="form-control"
                          value={form.password}
                          onChange={handleChange}
                          required
                          minLength={6}
                          placeholder="6 caracteres min."
                        />
                      </div>
                    </div>
                    <div className="col-6">
                      <label className="form-label small fw-semibold">Confirmer *</label>
                      <input
                        type="password"
                        name="confirmPassword"
                        className="form-control"
                        value={form.confirmPassword}
                        onChange={handleChange}
                        required
                      />
                    </div>
                  </div>

                  <div className="row g-3 mt-0">
                    <div className="col-6">
                      <label className="form-label small fw-semibold">Telephone</label>
                      <input type="tel" name="telephone" className="form-control" value={form.telephone} onChange={handleChange} />
                    </div>
                    <div className="col-6">
                      <label className="form-label small fw-semibold">Societe</label>
                      <input type="text" name="societe" className="form-control" value={form.societe} onChange={handleChange} />
                    </div>
                  </div>

                  <button type="submit" className="btn btn-primary w-100 py-2 mt-4 fw-bold" disabled={loading}>
                    {loading ? (
                      <><span className="spinner-border spinner-border-sm me-2"></span>Inscription en cours...</>
                    ) : (
                      <><i className="bi bi-check-circle me-2"></i>S'inscrire</>
                    )}
                  </button>
                </form>

                <p className="text-center text-muted mt-4 mb-0 small">
                  Deja un compte ? <Link to="/login" className="text-primary fw-semibold">Se connecter</Link>
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
