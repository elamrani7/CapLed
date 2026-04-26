import React, { useState } from 'react';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';

export const ContactPage = () => {
  const [formData, setFormData] = useState({
    nom: '',
    societe: '',
    email: '',
    telephone: '',
    objet: 'Demande de devis',
    message: ''
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Simulate sending
    alert('Message envoyé avec succès ! Notre équipe vous contactera sous peu.');
    setFormData({ ...formData, message: '' });
  };

  return (
    <div className="d-flex flex-column min-vh-100 bg-light">
      <Navbar />

      <main className="flex-grow-1 container py-5">
        <h1 className="fw-bolder text-dark mb-3">Contactez-nous</h1>
        <p className="text-secondary mb-5" style={{ maxWidth: '600px', fontSize: '1.05rem' }}>
          Une référence urgente, une pièce critique introuvable ou une commande récurrente ? Notre équipe répond rapidement.
        </p>

        <div className="row g-5">
          {/* Left side: Contact Info */}
          <div className="col-lg-5">
            <div className="d-flex flex-column gap-4">
              <div className="d-flex align-items-start gap-3">
                <div className="bg-primary text-white rounded d-flex align-items-center justify-content-center flex-shrink-0" style={{ width: '48px', height: '48px', fontSize: '1.25rem' }}>
                  <i className="bi bi-telephone-fill"></i>
                </div>
                <div>
                  <h6 className="fw-bold mb-1">Téléphone</h6>
                  <p className="text-secondary mb-0">07 02 380 380</p>
                </div>
              </div>

              <div className="d-flex align-items-start gap-3">
                <div className="bg-primary text-white rounded d-flex align-items-center justify-content-center flex-shrink-0" style={{ width: '48px', height: '48px', fontSize: '1.25rem' }}>
                  <i className="bi bi-clock-fill"></i>
                </div>
                <div>
                  <h6 className="fw-bold mb-1">Horaires</h6>
                  <p className="text-secondary mb-0">
                    Lun–Ven : 9h00 – 18h00<br />
                    Sam : 9h00 – 13h00
                  </p>
                </div>
              </div>

              <div className="d-flex align-items-start gap-3">
                <div className="bg-primary text-white rounded d-flex align-items-center justify-content-center flex-shrink-0" style={{ width: '48px', height: '48px', fontSize: '1.25rem' }}>
                  <i className="bi bi-envelope-fill"></i>
                </div>
                <div>
                  <h6 className="fw-bold mb-1">Email</h6>
                  <p className="text-secondary mb-0">
                    info@partfinder.ma<br />
                    <small>Réponse sous 2h ouvrées</small>
                  </p>
                </div>
              </div>

              <div className="d-flex align-items-start gap-3">
                <div className="bg-primary text-white rounded d-flex align-items-center justify-content-center flex-shrink-0" style={{ width: '48px', height: '48px', fontSize: '1.25rem' }}>
                  <i className="bi bi-geo-alt-fill"></i>
                </div>
                <div>
                  <h6 className="fw-bold mb-1">Casablanca</h6>
                  <p className="text-secondary mb-0">31, Lot. Salama II, Florida, Casablanca</p>
                </div>
              </div>

              <div className="d-flex align-items-start gap-3">
                <div className="bg-primary text-white rounded d-flex align-items-center justify-content-center flex-shrink-0" style={{ width: '48px', height: '48px', fontSize: '1.25rem' }}>
                  <i className="bi bi-geo-alt-fill"></i>
                </div>
                <div>
                  <h6 className="fw-bold mb-1">Tanger</h6>
                  <p className="text-secondary mb-0">Route de Tétouan, Lot 10 ZAE Bni Ouassine, Tanger</p>
                </div>
              </div>
            </div>
          </div>

          {/* Right side: Contact Form */}
          <div className="col-lg-7">
            <div className="card shadow-sm border-0" style={{ borderRadius: '12px' }}>
              <div className="card-body p-4 p-md-5">
                <h4 className="fw-bold mb-4">Envoyer un message</h4>
                
                <form onSubmit={handleSubmit}>
                  <div className="row g-3">
                    <div className="col-md-6">
                      <label className="form-label fw-semibold" style={{ fontSize: '0.9rem' }}>Nom complet <span className="text-danger">*</span></label>
                      <input type="text" className="form-control" placeholder="Votre nom" required value={formData.nom} onChange={e => setFormData({...formData, nom: e.target.value})} />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label fw-semibold" style={{ fontSize: '0.9rem' }}>Société</label>
                      <input type="text" className="form-control" placeholder="Entreprise" value={formData.societe} onChange={e => setFormData({...formData, societe: e.target.value})} />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label fw-semibold" style={{ fontSize: '0.9rem' }}>Email <span className="text-danger">*</span></label>
                      <input type="email" className="form-control" placeholder="votre@email.com" required value={formData.email} onChange={e => setFormData({...formData, email: e.target.value})} />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label fw-semibold" style={{ fontSize: '0.9rem' }}>Téléphone <span className="text-danger">*</span></label>
                      <input type="tel" className="form-control" placeholder="07 02 380 380" required value={formData.telephone} onChange={e => setFormData({...formData, telephone: e.target.value})} />
                    </div>
                    <div className="col-12">
                      <label className="form-label fw-semibold" style={{ fontSize: '0.9rem' }}>Objet</label>
                      <select className="form-select" value={formData.objet} onChange={e => setFormData({...formData, objet: e.target.value})}>
                        <option value="Demande de devis">Demande de devis</option>
                        <option value="Information produit">Information produit</option>
                        <option value="Suivi de commande">Suivi de commande</option>
                        <option value="Autre">Autre</option>
                      </select>
                    </div>
                    <div className="col-12">
                      <label className="form-label fw-semibold" style={{ fontSize: '0.9rem' }}>Message <span className="text-danger">*</span></label>
                      <textarea className="form-control" rows={5} placeholder="Références recherchées, quantités, contexte d'utilisation..." required value={formData.message} onChange={e => setFormData({...formData, message: e.target.value})}></textarea>
                    </div>
                    <div className="col-12 mt-4">
                      <button type="submit" className="btn btn-primary w-100 fw-bold py-2" style={{ borderRadius: '8px' }}>
                        <i className="bi bi-send-fill me-2"></i>Envoyer
                      </button>
                    </div>
                  </div>
                </form>

              </div>
            </div>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  );
};
