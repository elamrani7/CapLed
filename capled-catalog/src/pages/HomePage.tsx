import React from 'react';
import { Link } from 'react-router-dom';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';

export const HomePage = () => {
  return (
    <div className="d-flex flex-column min-vh-100 bg-light">
      <Navbar />
      <main className="flex-grow-1 container d-flex flex-column align-items-center justify-content-center text-center py-5">
        <h1 className="display-4 fw-bold text-dark mb-4">Bienvenue sur CapLed Catalogue</h1>
        <p className="lead text-secondary mb-5 max-w-75">
          Découvrez notre gamme complète d'équipements industriels neufs et reconditionnés.
          Construisez vos demandes de devis rapidement et facilement.
        </p>
        <Link to="/catalogue" className="btn btn-primary btn-lg shadow-sm">
          Accéder au Catalogue Public <i className="bi bi-arrow-right ms-2"></i>
        </Link>
      </main>
      <Footer />
    </div>
  );
};
