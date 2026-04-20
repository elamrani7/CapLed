import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { catalogueApi } from '../api/catalogueApi';
import { useCart } from '../context/CartContext';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';
import { Loader } from '../components/shared/Loader';
import { BadgeDisponibilite, BadgeCondition } from '../components/shared/Badge';
import { ProductImageGallery } from '../components/catalogue/ProductImageGallery';
import { EavTable } from '../components/catalogue/EavTable';
import { EtatDetailCard } from '../components/catalogue/EtatDetailCard';
import { ProductCard } from '../components/catalogue/ProductCard';

export const ProductDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();
  
  const [product, setProduct] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  const [showToast, setShowToast] = useState(false);

  useEffect(() => {
    const fetchProduct = async () => {
      setLoading(true);
      setError('');
      try {
        const res = await catalogueApi.getProductById(Number(id));
        setProduct(res);
      } catch (err: any) {
        setError(err.message || "Article introuvable.");
      } finally {
        setLoading(false);
      }
    };
    
    if (id) {
      fetchProduct();
      window.scrollTo(0, 0);
    }
  }, [id]);

  const handleAddToCart = () => {
    if (!product) return;
    addToCart({
      id: product.id,
      articleId: product.id,
      nom: product.nom,
      prixVente: product.prixVente,
      reference: product.reference,
      image: product.urlImagePrincipale || (product.images?.length > 0 ? product.images[0] : null)
    }, 1);
    
    setShowToast(true);
    setTimeout(() => setShowToast(false), 3000);
  };

  if (loading) {
    return (
      <div className="d-flex flex-column min-vh-100 bg-light">
        <Navbar />
        <main className="flex-grow-1 d-flex align-items-center justify-content-center">
          <Loader message="Chargement de l'article..." />
        </main>
        <Footer />
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="d-flex flex-column min-vh-100 bg-light">
        <Navbar />
        <main className="flex-grow-1 container d-flex flex-column align-items-center justify-content-center">
          <h2 className="text-danger fw-bold mb-3">Erreur</h2>
          <p className="text-secondary mb-4">{error || "Cet article n'existe plus."}</p>
          <button onClick={() => navigate('/catalogue')} className="btn btn-link text-primary">
            &larr; Retour au catalogue
          </button>
        </main>
        <Footer />
      </div>
    );
  }

  const isOccasionOrRecond = product.condition === 'OCCASION' || product.condition === 'RECONDITIONNE';

  return (
    <div className="d-flex flex-column min-vh-100 bg-light pb-5">
      <Navbar />

      {/* Toast Notification Bootstrap */}
      <div className="toast-container position-fixed top-0 end-0 p-3 mt-5" style={{ zIndex: 1055 }}>
        <div className={`toast align-items-center text-bg-success border-0 ${showToast ? 'show' : ''}`} role="alert" aria-live="assertive" aria-atomic="true">
          <div className="d-flex">
            <div className="toast-body fw-bold">
              <i className="bi bi-check-circle-fill me-2"></i>
              Article ajouté au panier !
            </div>
          </div>
        </div>
      </div>

      <div className="container-fluid px-4 py-4">
        <button onClick={() => navigate(-1)} className="btn btn-link link-secondary text-decoration-none p-0 mb-3 d-flex align-items-center gap-2 fw-bold" style={{ fontSize: '0.9rem' }}>
          <i className="bi bi-arrow-left"></i> Retour aux résultats
        </button>

        <div className="row g-4">
          {/* Section Image */}
          <div className="col-lg-4 col-xl-3">
            <ProductImageGallery images={product.images} reference={product.reference} />
          </div>

          {/* Section Infos */}
          <div className="col-lg-8 col-xl-9">
            <div className="mb-3">
              <div className="d-flex flex-wrap justify-content-between align-items-center mb-1">
                <p className="small text-muted font-monospace text-uppercase mb-0 fw-bold">RÉF : {product.reference}</p>
                <div className="d-flex gap-1" style={{ scale: '0.9', transformOrigin: 'right' }}>
                  <BadgeDisponibilite dispo={product.disponibiliteBadge} />
                  <BadgeCondition condition={product.condition} grade={product.etatDetail?.gradeVisuel} />
                </div>
              </div>
              <h1 className="fw-bolder text-dark mb-0 lh-sm" style={{ fontSize: '1.75rem' }}>{product.nom}</h1>
            </div>

            <div className="row g-4">
              
              <div className="col-xl-8">
                {/* Nav Tabs for Info - Auto-Doc Style Layout */}
                <ul className="nav nav-tabs mb-3 border-bottom-0" id="productTab" role="tablist">
                  <li className="nav-item" role="presentation">
                    <button className="nav-link active fw-bold text-dark border-bottom-0 bg-white" style={{ borderTop: '3px solid #ff7a00' }} id="specs-tab" data-bs-toggle="tab" data-bs-target="#specs" type="button" role="tab">Caractéristiques Techniques</button>
                  </li>
                </ul>
                
                <div className="tab-content border-top pt-3" id="productTabContent">
                  <div className="tab-pane fade show active" id="specs" role="tabpanel">
                    {product.description && (
                      <div className="mb-4 text-secondary lh-lg p-3 bg-white border rounded" style={{ fontSize: '0.9rem' }}>
                        {product.description}
                      </div>
                    )}
                    
                    <div className="mb-4">
                      <EavTable specs={product.caracteristiquesPrincipales} />
                    </div>

                    {isOccasionOrRecond && product.etatDetail && (
                      <div className="mb-4 border-top pt-3">
                        <h5 className="fw-bold mb-2 d-flex align-items-center gap-2" style={{ fontSize: '1.1rem' }}><i className="bi bi-clipboard-check text-success"></i> Rapport d'état</h5>
                        <EtatDetailCard etatDetail={product.etatDetail} />
                      </div>
                    )}
                  </div>
                </div>
              </div>

              <div className="col-xl-4">
                {/* B2B CTA Box */}
                <div className="card shadow-sm border-0 bg-white mb-4 rounded-0" style={{ outline: '1px solid #e9ecef', position: 'sticky', top: '20px' }}>
                  <div className="card-body p-3 p-xl-4 d-flex flex-column gap-3">
                    <div className="mb-1">
                      <div className="d-flex align-items-center gap-2 mb-2">
                        <i className="bi bi-tag-fill text-primary fs-4"></i>
                        <span className="fs-4 fw-bolder text-primary lh-1">Prix sur devis</span>
                      </div>
                      <span className="small text-muted d-block" style={{ fontSize: '0.8rem' }}>Ajoutez cet article à votre demande de devis pour recevoir nos meilleurs tarifs.</span>
                      <span className="small text-success fw-bold d-block mt-2" style={{ fontSize: '0.8rem' }}><i className="bi bi-truck me-1"></i>Livraison disponible</span>
                    </div>
                    
                    <button 
                      onClick={handleAddToCart}
                      className="btn btn-warning w-100 shadow-sm fw-bolder py-2 d-flex justify-content-center align-items-center gap-2 border-0 text-dark"
                      style={{ fontSize: '1.1rem' }}
                    >
                      <i className="bi bi-cart-plus me-1"></i>
                      Ajouter au devis
                    </button>
                  </div>
                </div>
              </div>

            </div>
          </div>
        </div>

        {/* Similar Products */}
        {product.articlesSimilaires && product.articlesSimilaires.length > 0 && (
          <section className="mt-4 pt-4 border-top">
            <h4 className="fw-bold mb-3 fs-5">Produits Similaires</h4>
            <div className="row row-cols-1 row-cols-sm-2 row-cols-md-4 row-cols-xl-5 g-3">
              {product.articlesSimilaires.map((sim: any) => (
                <div className="col" key={sim.id}>
                  <ProductCard product={sim} />
                </div>
              ))}
            </div>
          </section>
        )}
      </div>

      <Footer />
    </div>
  );
};
