import React, { useState } from 'react';

type ProductImageGalleryProps = {
  images?: string[];
  reference: string;
};

export const ProductImageGallery = ({ images, reference }: ProductImageGalleryProps) => {
  const [activeIndex, setActiveIndex] = useState(0);

  if (!images || images.length === 0) {
    return (
      <div className="bg-white rounded shadow-sm d-flex align-items-center justify-content-center border" style={{ height: '400px' }}>
        <span className="text-muted fw-medium fs-5"><i className="bi bi-image"></i> Image non disponible</span>
      </div>
    );
  }

  const mainImage = images[activeIndex];

  return (
    <div className="row g-3 flex-column-reverse flex-lg-row">
      {/* Miniatures */}
      {images.length > 1 && (
        <div className="col-lg-2 d-flex flex-row flex-lg-column gap-2 overflow-auto" style={{ maxHeight: '400px' }}>
          {images.map((img, idx) => (
            <button
              key={idx}
              onClick={() => setActiveIndex(idx)}
              className={`p-0 bg-white rounded overflow-hidden flex-shrink-0 ${
                activeIndex === idx ? 'border border-primary border-2 opacity-100' : 'border border-transparent opacity-50'
              }`}
              style={{ width: '80px', height: '80px', transition: 'all 0.2s' }}
            >
              <img src={`https://capled-api.onrender.com${img}`} alt={`${reference} vue ${idx + 1}`} className="w-100 h-100 object-fit-cover" />
            </button>
          ))}
        </div>
      )}

      {/* Image Principale */}
      <div className="col flex-grow-1">
        <div className="bg-white border rounded overflow-hidden shadow-sm d-flex align-items-center justify-content-center p-3" style={{ height: '400px' }}>
          <img 
            src={`https://capled-api.onrender.com${mainImage}`} 
            alt={`Aperçu ${reference}`} 
            className="w-100 h-100 object-fit-contain"
          />
        </div>
      </div>
    </div>
  );
};
