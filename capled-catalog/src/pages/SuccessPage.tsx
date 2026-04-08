import React from 'react';
import { Link } from 'react-router-dom';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';

export const SuccessPage = () => {
  return (
    <div className="d-flex flex-column min-vh-100 bg-light">
      <Navbar />
      <main className="flex-grow-1 container d-flex flex-column align-items-center justify-content-center text-center py-5">
        <div className="card shadow-sm border-0 p-5 rounded-4" style={{ maxWidth: '600px', width: '100%' }}>
          <i className="bi bi-check-circle-fill text-success" style={{ fontSize: '5rem' }}></i>
          <h2 className="display-6 fw-bold text-dark mt-4 mb-3">Demande Envoyée !</h2>
          <p className="fs-5 text-secondary mb-5">
            Votre demande de devis a bien été transmise à notre équipe. Nous vous recontacterons dans les plus brefs délais.
          </p>
          <Link to="/catalogue" className="btn btn-primary btn-lg px-5 shadow-sm fw-medium">
            Retour au catalogue
          </Link>
        </div>
      </main>
      <Footer />
    </div>
  );
};
