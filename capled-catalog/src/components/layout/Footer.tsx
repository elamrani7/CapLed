import React from 'react';

export const Footer = () => {
  return (
    <footer className="bg-dark text-light py-4 mt-auto">
      <div className="container text-center">
        <p className="mb-0">
          &copy; {new Date().getFullYear()} CapLed. Tous droits réservés.
        </p>
        <p className="text-secondary small mt-1">
          Propulsé par CapLed ERP
        </p>
      </div>
    </footer>
  );
};
